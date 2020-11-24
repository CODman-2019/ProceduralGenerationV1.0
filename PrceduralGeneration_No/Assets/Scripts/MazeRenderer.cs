using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeRenderer : MonoBehaviour
{
    [SerializeField]
    private Transform wallPrefab = null;

    [SerializeField]
    private Transform floorObject = null;

    public Transform FPSController;
    public Transform teleporter;

    [Range(1, 50)]
    public int width;
    [Range(1, 50)]
    public int height;

    [Range(1, 50)]
    public int levels;

    [SerializeField]
    private float size = 1f;



    // Start is called before the first frame update
    void Start()
    {
        //WallState[] mazes = new WallState[levels];
        var mazes = new List<WallState[,]>();

        //for (int x = 0; x < levels; x++)
        //{
        //    //var maze = 
        //    mazes[x] = MazeGenerator.Generate(width, height, true);;
        //}
        var maze = MazeGenerator.Generate(width, height, true);
        Draw(maze);

        //var rnd = new System.Random();

    }

    //builds maze
    private void Draw(WallState[,] maze)
    {

        //creating the floor
        var floor = Instantiate(floorObject);
        floor.localScale = new Vector3(width, 0.1f, height);

        //go through the width and height
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j< height; j++)
            {
                //create a cell
                var cell = maze[i, j];
                //adjust the position
                var position = new Vector3(-width / 2 + i, 0, -height / 2 + j);

                //creating and instantiating walls based on which wall is made
                if (cell.HasFlag(WallState.UP))
                {

                    var topWall = Instantiate(wallPrefab, transform);
                    topWall.position = position + new Vector3(0, 0, size/2);
                    topWall.localScale = new Vector3(size, topWall.localScale.y, topWall.localScale.z);
                }

                if (cell.HasFlag(WallState.LEFT))
                {
                    var leftWall = Instantiate(wallPrefab, transform);
                    leftWall.position = position + new Vector3(-size / 2, 0, 0);
                    leftWall.localScale = new Vector3(size, leftWall.localScale.y, leftWall.localScale.z);
                    leftWall.eulerAngles = new Vector3(0, 90, 0);
                }

                //if the loop reaches the las collum it builds the right wall
                if(i == width - 1)
                {
                    if (cell.HasFlag(WallState.RIGHT))
                    {
                        var rightWall = Instantiate(wallPrefab, transform);
                        rightWall.position = position + new Vector3(size / 2, 0, 0);
                        rightWall.localScale = new Vector3(size, rightWall.localScale.y, rightWall.localScale.z);
                        rightWall.eulerAngles = new Vector3(0, 90, 0);
                    }
                }

                //if the loop is on the first collom, it builds the bottom wall
                if (j == 0)
                {
                    if (cell.HasFlag(WallState.DOWN))
                    {
                        var bottomWall = Instantiate(wallPrefab, transform);
                        bottomWall.position = position + new Vector3(0, 0, -size / 2);
                        bottomWall.localScale = new Vector3(size, bottomWall.localScale.y, bottomWall.localScale.z);
                    }
                }

                if (cell.HasFlag(WallState.PLAYER))
                {
                    var player = Instantiate(FPSController, transform);
                    //player.position = position;
                }

                //if (cell.HasFlag(WallState.TELEPORTER))
                //{
                //    var teleport = Instantiate(teleporter, transform) as Transform;
                //}

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    var maze = MazeGenerator.Generate(width, height, true);
        //    Draw(maze);
        //}   
    }
}









// as Transform