using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;

public class ControlFaust : MonoBehaviour
{   
    public FaustPlugin_suka_final thePlygin;
    public float velScaling;
    public float weightSeconds;

    private float _velocity;
    private float _pitch;
    private float _gain;
    private float _time;

    
    private bool isSeenFlag;

    private Vector _prevPosElbow;
    private Vector _prevPosWrist;

    private List<float> _weightVelocity = new List<float> {0.0f};
    private float _weightEffort = 0;
    private float prevTime;

    private Hand _handR;
    private Hand _handL;
    
    // filtering parameters
    private float[] _prevVel;
    private float[] _prevPinch;
    private float[] _prevElbowVel;
    private float[] _prevWristVel;
    // Start is called before the first frame update
    void Start()
    {
        // initiating time
        prevTime = Time.time;

        // initiating filter parameters
        _prevVel = new float[4] {0.0f, 0.0f, 0.0f, 0.0f};
        _prevElbowVel = new float[4] {0.0f, 0.0f, 0.0f, 0.0f};
        _prevWristVel = new float[4] {0.0f, 0.0f, 0.0f, 0.0f};
        thePlygin.setParameter(0, 1);
    }

    // Update is called once per frame
    void Update()
    {   
        _handR = Hands.Right;
        _handL = Hands.Left;
        
        // Right hand control
        if(_handR != null)
        {
            // setting bowing velocity through 
            // the velocity of the left palm
            _velocity = smooFilter(_prevVel, _handR.PalmVelocity.Magnitude);
            // thePlygin.setParameter(0, _velocity*velScaling);
            thePlygin.setParameter(0,weightEffort(_handR)*velScaling);
            isSeenFlag = true;
        }
        else
        {
            isSeenFlag = false;
        }

        // setting gain value
        if(_handL != null)
        {
            _gain = pinching();
            thePlygin.setParameter(5, _gain);

            _pitch = normal();
            thePlygin.setParameter(4, _pitch);

        }

    }

    //Mapping function

    // handling and mapping pinching
    private float pinching()
    {
        return mapping (_handL.PinchDistance, 85.0f, 5.0f, -96.0f, 0.0f);
    }


    // handling and mapping normal 

    private float normal()
    {
        return mapping(_handL.PalmNormal.Roll, -3.14f, 3.14f, 300.0f, 1000.0f);
    }

    private float weightEffort(Hand _hand)
    {   
        Vector velElbow; 
        Vector velWrist;
        float[] array;

        Vector _currPosElbow = _hand.Arm.ElbowPosition;
        Vector _currPosWrist = _hand.Arm.WristPosition;

        _prevPosElbow = _currPosElbow;
        _prevPosWrist = _currPosWrist;
        // calculate velocity for each joint
        if(isSeenFlag)
        {

            
            velElbow = (_currPosElbow - _prevPosElbow)/Time.deltaTime;
            velWrist = (_currPosWrist - _prevPosWrist)/Time.deltaTime;
            
            // filtering 
            float velElbowMag = smooFilter(_prevElbowVel, velElbow.Magnitude);
            float velWristMag = smooFilter(_prevElbowVel, velWrist.Magnitude);

            

            // sqare and add together
            // put in a list
            // devide by the number of joints 
            _weightVelocity.Add((
                velElbowMag*velElbowMag +
                velWristMag*velWristMag +
                _velocity*_velocity)
                /3.0f);

            // after curtain time get the max value of the velocity 
            if(Time.time - prevTime >= weightSeconds)
            {   
                prevTime = Time.time;
                array = _weightVelocity.ToArray();
                _weightEffort = Mathf.Max(array);
                _weightVelocity.Clear();
            }
        }
            
        return _weightEffort;

        
    }


    // Utility functions

    // smoothing 5th order average filter
    private float smooFilter(float[] prev, float magnitude)
    {   
        float sum = 0;

        for(int i = 0; i < 4; i++)
            sum += prev[i];

        float value = (magnitude + sum)*0.2f;

        for(int i = 0; i < 3; i++)
            prev[i] = prev[i+1];

        prev[3] = value;

        return value;
    }

    // mapping function
    private float mapping(float val, float in_min, float in_max, float out_min, float out_max)
    {
        return (val - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }


}
