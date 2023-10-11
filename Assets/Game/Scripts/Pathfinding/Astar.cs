using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Andremani.DemoHexBattle.Pathfinding.Heuristics;

namespace Andremani.DemoHexBattle.Pathfinding
{
    public abstract class Astar
    {
        private Heuristic heuristic;

        public Heuristic Heuristic => heuristic;

        public Astar(Heuristic heuristic)
        {
            this.heuristic = heuristic;
        }

        public abstract List<Node> CreatePath(List<Vector3Int> grid, Vector3Int start, Vector3Int end, int length);

        protected List<Node> FindPath(Node startNode, Node endNode, int length)
        {
            if (!IsValidPath(startNode, endNode))
                return null;
            List<Node> openSet = new List<Node>();
            List<Node> closedSet = new List<Node>();

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                //Find shortest step distance in the direction of your goal within the open set
                int winner = 0;
                for (int i = 0; i < openSet.Count; i++)
                    if (openSet[i].F < openSet[winner].F)
                        winner = i;
                    else if (openSet[i].F == openSet[winner].F)//tie breaking for faster routing
                        if (openSet[i].H < openSet[winner].H)
                            winner = i;

                var current = openSet[winner];

                //Found the path, creates and returns the path
                if (endNode != null && openSet[winner] == endNode)
                {
                    List<Node> Path = new List<Node>();
                    var temp = current;
                    Path.Add(temp);
                    while (temp.previous != null)
                    {
                        Path.Add(temp.previous);
                        temp = temp.previous;
                    }
                    if (length - (Path.Count - 1) < 0)
                    {
                        //Path.RemoveRange(0, (Path.Count - 1) - length); //trim path to length
                        return null;
                    }
                    Path.Reverse();
                    return Path;
                }

                openSet.Remove(current);
                closedSet.Add(current);


                //Finds the next closest step on the grid
                var neighboors = current.neighboors;
                for (int i = 0; i < neighboors.Count; i++)//look threw our current spots neighboors (current spot is the shortest F distance in openSet
                {
                    var n = neighboors[i];
                    if (!closedSet.Contains(n) && n.height < 1)//Checks to make sure the neighboor of our current tile is not within closed set, and has a height of less than 1
                    {
                        var tempG = current.G; //gets a temp comparison integer for seeing if a route is shorter than our current path
                        if (heuristic.HeuristicType == HeuristicTypes.SquareGrid)
                        {
                            if (n.X != current.X && n.Y != current.Y) //if current->n is diagonal
                            {
                                tempG += heuristic.DiagonalMoveCost; // diagonal movement cost
                            }
                            else
                            {
                                tempG += heuristic.DefaultMoveCost; // straight movement cost
                            }
                        }
                        else
                        {
                            tempG += heuristic.DefaultMoveCost;
                        }

                        bool newPath = false;
                        if (openSet.Contains(n)) //Checks if the neighboor we are checking is within the openset
                        {
                            if (tempG < n.G)//The distance to the end goal from this neighboor is shorter so we need a new path
                            {
                                n.G = tempG;
                                newPath = true;
                            }
                        }
                        else//if its not in openSet or closed set, then it IS a new path and we should add it too openset
                        {
                            n.G = tempG;
                            newPath = true;
                            openSet.Add(n);
                        }
                        if (newPath)//if it is a newPath caclulate the H and F and set current to the neighboors previous
                        {
                            n.H = heuristic.GetHeuristic(n, endNode);
                            n.F = n.G + n.H;
                            n.previous = current;
                        }
                    }
                }

            }
            return null;
        }

        private bool IsValidPath(Node start, Node end)
        {
            if (end == null)
                return false;
            if (start == null)
                return false;
            if (end.height >= 1)
                return false;
            return true;
        }
    }

    public class Node
    {
        public int X;
        public int Y;
        public int F;
        public int G; // real graph distance (from start)
        public int H; // heuristic distance
        public int height = 0;
        public List<Node> neighboors;
        public Node previous = null;

        public Node(int x, int y, int _height)
        {
            X = x;
            Y = y;
            F = 0;
            G = 0;
            H = 0;
            neighboors = new List<Node>();
            height = _height;
        }
    }
}