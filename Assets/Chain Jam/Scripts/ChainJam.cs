using UnityEngine;
using System.Collections.Generic;
using System.Collections;

using System;

public class ChainJam : MonoBehaviour {
	
	/*
		These are the things you need to implement:
		
		GAME:
		
		void GameEnd():
			end the game before time. If this is never called, your game will automatically finish after 1 minute.
		
		void ChainJam.AddFunctionBeforeExit(Action function, int s)
			will execute the “function”, “s” seconds before the 1 minute timeframe is over.
			Use this if you want to end your game 'nicely', with a fade or an animation.

			Example: 
				if you have a void Fadeout() function:
				ChainJam.AddFunctionBeforeExit(Fadeout,5);  // this will execute fadeout 5 seconds before the game ends.
			If your function has parameters, use () => function

			Example:
				if you have a void Fadeout(int length) function:
				ChainJam.AddFunctionBeforeExit(() => Fadeout(5),5);  
							
		ENUMS:

		enum ChainJam.BUTTON: 
			you will need this enum to specify a button.
			
			
		SCORE:	
			
		void ChainJam.AddPoints(int p): 
			use this to add “p” points.

		int ChainJam.GetPoints():
			returns the points distributed during this game.
			
			
		CONTROLLERS: 	
			
		bool ChainJam.GetButtonPressed(ChainJam.BUTTON button): 
			will return true or false depending on whether the “button” is pressed for player.
			
		bool ChainJam.GetButtonJustPressed(ChainJam.BUTTON button): 
			will return true only in the frame where the “button” is pressed for player. 
			
		bool ChainJam.GetButtonJustReleased(ChainJam.BUTTON button): 
			will return true only in the frame where the “button” is released for player. 
			
	*/
	
	
	// Controller enums
	public enum BUTTON {LEFT,RIGHT,UP,DOWN,A,B};
	
	// Private vars
	private static int _player1Points = 0;
	private static float _timePassed = 0;
	private static float _timePassedLast = -1;

	private Dictionary<int,List<Action>> _actions;
	
	
	void Awake () {
		DontDestroyOnLoad(this.gameObject);
		_actions = new Dictionary<int, List<Action>>();
		GameStart();
	}
	
	public void AddFunctionBeforeExit(Action function, int s)
	{
		List<Action> list = new List<Action>();
		list.Add(function);
		
		if(!_actions.ContainsKey(s))
		{
			_actions.Add(s,list);
		}
		else
		{
			_actions[s].AddRange(list);
		}
	}
		
	void Update() {
		_timePassed += Time.deltaTime;
		
		int index = (int)Mathf.Round(60-_timePassed);
		int lastIindex = (int)Mathf.Round(60-_timePassedLast);
		if(index != lastIindex)
		{
			_timePassedLast = _timePassed;
			if (_actions.ContainsKey(index))
			{
				foreach (Action function in _actions[index]) {
					function();
				}
			}
		}
	}

	public static bool GetButtonPressed(BUTTON button)
	{
		return Input.GetKey(GetKeycode(button));
	}
	
	public static bool GetButtonJustPressed(BUTTON button)
	{
		return Input.GetKeyDown(GetKeycode(button));
	}
	
	public static bool GetButtonJustReleased(BUTTON button)
	{
		return Input.GetKeyUp(GetKeycode(button));
	}	

	private static KeyCode GetKeycode(BUTTON button)
	{
		KeyCode key = KeyCode.Space;
		switch(button)
		{
		case BUTTON.A:
			key = KeyCode.X;
			break;
		case BUTTON.B:
			key = KeyCode.C;
			break;
		case BUTTON.LEFT:
			key = KeyCode.LeftArrow;
			break;
		case BUTTON.RIGHT:
			key = KeyCode.RightArrow;
			break;
		case BUTTON.UP:
			key = KeyCode.UpArrow;
			break;
		case BUTTON.DOWN:
			key = KeyCode.DownArrow;
			break;
		}	
		return key;
	}
	
	
	// call to tell the Mini Game Machine that the game has ended
	public static void GameEnd()
	{
		Application.ExternalCall("GameEnd", "");
	}
	
	private static void GameStart()
	{
		Application.ExternalCall("GameStart", "");
	}
	
	// adds points to a given player
	public static void AddPoints(int points)
	{
		Player1Points += points;
	}

	// remove points to a given player
	public static void RemovePoints(int points)
	{
		Player1Points -= points;
	}

	public static int GetPoints()
	{
		return Player1Points;
	}
	
	private static int Player1Points
	{
		get {return _player1Points;}
		set 
		{
			int points = value - _player1Points;
			if (value >= 0 && value <10) 
			{
				Application.ExternalCall("PlayerOnePoints", points);
				_player1Points = value;
			}
		}
	}
}
