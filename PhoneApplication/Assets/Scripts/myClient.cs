// ===============================
// AUTHOR     : Rafael Maio (rafael.maio@ua.pt)
// PURPOSE     : Handles the client side on client-server model.
// SPECIAL NOTES: 
// ===============================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using System.IO;    

public class myClient
{
    /// <summary>
    /// Creates the socket TCP
    /// </summary>
    public Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    /// <summary>
    /// Declare end point.
    /// </summary>
    public IPEndPoint conn;

    /// <summary>
    /// Message to be sent.
    /// </summary>
    public Message message;

    /// <summary>
    /// Server host address.
    /// </summary>
    public string server_host_address = "192.168.1.103";

    /// <summary>
    /// Server port.
    /// </summary>
    public int server_port = 4444;

    /// <summary>
    /// Constructor - Sets the server address and port.
    /// Creates a default message.
    /// </summary>
    /// <param name="username">Player name.</param>
    /// <param name="_host_address">Server host address.</param>
    /// <param name="_port">Server port.</param>
    public myClient(string username, string _host_address = "192.168.43.113", int _port = 4444)
    {
        server_host_address = _host_address;
        server_port = _port;
        message = new Message(username, DateTime.Now);
    }

    /// <summary>
    /// Connects to sever.
    /// </summary>
    /// <returns>If the connection succeeded.</returns>
    public bool ConnectToServer()
    {
        //If already connected ignores this function.
        if (clientSocket.Connected)
        {
            return false;
        }

        //Try to connect the socket to the server.
        try
        {
            //Create end point to connect
            conn = new IPEndPoint(IPAddress.Parse(server_host_address), server_port);
            //Connect to server
            clientSocket.BeginConnect(conn, ConnectCallback, clientSocket);

        }
        catch (Exception ex)
        {
            Debug.Log("socket error: " + ex.Message);
        }

        return clientSocket.Connected;
    }

    /// <summary>
    /// Async call to connect.
    /// </summary>
    static void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket.
            Socket client = (Socket)ar.AsyncState;

            // Complete the connection.
            client.EndConnect(ar);
        }
        catch (Exception e)
        {
            Debug.Log("Error connecting: " + e);
        }
    }

    /// <summary>
    /// Send data to server.
    /// </summary>
    public void SendData()
    {
        SendData(clientSocket);
    }

    /// <summary>
    /// Send data to server.
    /// </summary>
    /// <param name="client">Client sockect.</param>
    public void SendData(Socket client)
    {
        SendData(client, message.ToString());
    }

    /// <summary>
    /// Send data to server.
    /// </summary>
    /// <param name="client">Client sockect.</param>
    /// <param name="data">Data to be sent.</param>
    public static void SendData(Socket client, string data)
    {
        //Convert the string data to bytes.
        byte[] byteData = Encoding.ASCII.GetBytes(data);

        // Begin sending the data to server.  
        client.BeginSend(byteData, 0, byteData.Length, 0,
            new AsyncCallback(SendCallBack), client);
    }

    /// <summary>
    /// Async send data to server.
    /// </summary>
    static void SendCallBack(IAsyncResult ar)
    {
        try
        {
            Socket client = (Socket)ar.AsyncState;

            //Send data to the server.
            int bytesSent = client.EndSend(ar);
        }
        catch (Exception e)
        {
            Debug.Log("error sending message: " + e);
        }
    }

    /// <summary>
    /// Unity OnApplicationQuit - Close the socket.
    /// </summary>
    void OnApplicationQuit()
    {
        CloseSocket();
    }

    /// <summary>
    /// Unity OnDisabled - Close the socket.
    /// </summary>
    void OnDisable()
    {
        CloseSocket();
    }

    /// <summary>
    /// Close socket.
    /// </summary>
    public void CloseSocket()
    {
        if (!clientSocket.Connected)
        {
            return;
        }
        clientSocket.Close();
    }

    /// <summary>
    /// Message structure.
    /// </summary>
    [Serializable]
    public class Message
    {
        /// <summary>
        /// Player username.
        /// </summary>
        public string user_name;

        /// <summary>
        /// Time when the first message was sent.
        /// </summary>
        public DateTime initial_time;

        /// <summary>
        /// Time when the first message was sent in string format.
        /// </summary>
        public string initial_time_s;

        /// <summary>
        /// Message Type.
        /// Types: REGISTRATION, CAMERA, LOOKAWAY_SUCC, LOOKAWAY_FAIL, SPOTLIGHT_OBJ, STOP_SUCC, STOP_FAIL, STOP_OBJ, DODGE_OBJ, DODGE_FAIL, MOVING_OBJECT, STATIC, MOVING, LINE, END, OUT_OF_ROAD, HINT, GOAL, CHECKPOINT
        /// </summary>
        public string type;

        /// <summary>
        /// Message timestamp.
        /// </summary>
        public TimeSpan current_timestamp;

        /// <summary>
        /// Message timestamp in string format.
        /// </summary>
        public string current_timestamp_s;

        /// <summary>
        /// Pose class variable.
        /// </summary>
        public AuxPose obj;

        /// <summary>
        /// GameState class variable.
        /// </summary>
        public GameState gameState;

        /// <summary>
        /// Line positions.
        /// </summary>
        public Vector3[] line;

        /// <summary>
        /// Message boundary.
        /// </summary>
        public string messageEnd = "split_by_this_message_end";

        /// <summary>
        /// Message constructor - REGISTRATION.
        /// </summary>
        /// <param name="name">Player username.</param>
        /// <param name="time">Message date.</param>
        public Message(string name, DateTime time)
        {
            user_name = name;
            initial_time = time;
            initial_time_s = time.ToString();
            type = "REGISTRATION";
        }

        /// <summary>
        /// Set the pose of the device or moving object, the message type and the corresponding timestamp.
        /// </summary>
        /// <param name="_type">Message type.</param>
        /// <param name="position">Device/moving object position.</param>
        /// <param name="rotation">Device/moving object rotation.</param>
        /// <param name="timestamp">Message timestamp.</param>
        public void objCoords(string _type, Vector3 position, Vector3 rotation, DateTime timestamp)
        {
            type = _type;
            current_timestamp = timestamp - initial_time;
            current_timestamp_s = current_timestamp.ToString();
            obj = new AuxPose(position.x, position.y, position.z, rotation.x, rotation.y, rotation.z);
        }

        /// <summary>
        /// Set the game type - Static objects or moving object.
        /// </summary>
        /// <param name="_type">Message type.</param>
        public void setTypeOfGame(string _type)
        {
            type = _type;
        }

        /// <summary>
        /// Set the game state, objects pose, message type and corresponding timestamp.
        /// </summary>
        /// <param name="_type">Message type.</param>
        /// <param name="timestamp">Message timestamp.</param>
        /// <param name="position">Object position.</param>
        /// <param name="rotation">Object rotation.</param>
        /// <param name="num_checkpoints">Number of checkpoints caught at the corresponding timestamp.</param>
        /// <param name="score">Game score at the corresponding timestamp.</param>
        /// <param name="time">Game time at the corresponding timestamp.</param>
        public void setGameState(string _type, DateTime timestamp, Vector3 position, Vector3 rotation, int num_checkpoints, int score, float time)
        {
            type = _type;
            current_timestamp = timestamp - initial_time;
            current_timestamp_s = current_timestamp.ToString();
            obj = new AuxPose(position.x, position.y, position.z, rotation.x, rotation.y, rotation.z);
            gameState = new GameState(score, time, num_checkpoints);
        }

        /// <summary>
        /// Set the line points positions.
        /// </summary>
        /// <param name="_type">Message type.</param>
        /// <param name="timestamp">Message timestamp.</param>
        /// <param name="_line">Line points positions.</param>
        public void setLine(string _type, DateTime timestamp, Vector3[] _line)
        {
            type = _type;
            current_timestamp = timestamp - initial_time;
            current_timestamp_s = current_timestamp.ToString();
            line = _line;
        }

        /// <summary>
        /// Overrides ToString() method.
        /// </summary>
        /// <returns>Return the json string of this object.</returns>
        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }

    /// <summary>
    /// Game State class -> saves the state of the game at the current timestamp.
    /// </summary>
    [Serializable]
    public struct GameState
    {
        /// <summary>
        /// Game elapse minutes.
        /// </summary>
        public float minutes;

        /// <summary>
        /// Game elapse seconds.
        /// </summary>
        public float seconds;

        /// <summary>
        /// Number of checkpoints caught at the game.
        /// </summary>
        public int num_checkpoints;

        /// <summary>
        /// Game score.
        /// </summary>
        public int score;

        /// <summary>
        /// Game State constructor.
        /// </summary>
        /// <param name="_score">Game score.</param>
        /// <param name="_time">Game elapsed time.</param>
        /// <param name="_num_checkpoints">Number of checkpoints caught at the game.</param>
        public GameState(int _score, float _time, int _num_checkpoints)
        {
            score = _score;
            minutes = (int)(_time / 60);
            seconds = (int)(_time % 60);
            num_checkpoints = _num_checkpoints;
        }
    }

    /// <summary>
    /// Pose class -> saves the pose of an object.
    /// </summary>
    [Serializable]
    public struct AuxPose
    {
        /// <summary>
        /// Object x position.
        /// </summary>
        public float position_x;

        /// <summary>
        /// Object y position.
        /// </summary>
        public float position_y;

        /// <summary>
        /// Object z position.
        /// </summary>
        public float position_z;

        /// <summary>
        /// Object x rotation.
        /// </summary>
        public float rotation_x;

        /// <summary>
        /// Object y rotation.
        /// </summary>
        public float rotation_y;

        /// <summary>
        /// Object z rotation.
        /// </summary>
        public float rotation_z;

        /// <summary>
        /// Pose constructor.
        /// </summary>
        /// <param name="pos_x">Object x position.</param>
        /// <param name="pos_y">Object y position.</param>
        /// <param name="pos_z">Object z position.</param>
        /// <param name="rot_x">Object x rotation.</param>
        /// <param name="rot_y">Object y rotation.</param>
        /// <param name="rot_z">Object z rotation.</param>
        public AuxPose(float pos_x, float pos_y, float pos_z, float rot_x, float rot_y, float rot_z) 
        {
            position_x = pos_x;
            position_y = pos_y;
            position_z = pos_z;
            rotation_x = rot_x;
            rotation_y = rot_y;
            rotation_z = rot_z;
        }

        /// <summary>
        /// Overrides ToString() method.
        /// </summary>
        /// <returns>Return the json string of this object.</returns>
        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }
}