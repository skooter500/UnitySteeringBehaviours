using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BGE
{
    public class Space
    {
        public List<Cell> cells = new List<Cell>();
        float worldRadius;
        int spaceWidth;
        float cellWidth;
        public bool needsPartitioning = true;
        GameObject[] boids; 

        public Space()
        {            
            // generate the list of cells
            cellWidth = Params.GetFloat("cell_width");
            // Create an additional cell either side of the world range 
            worldRadius = Params.GetFloat("world_range") + Params.GetFloat("cell_width");

            int num = 0;

            float y = 0;
            {
                for (float z = -worldRadius; z < worldRadius; z += cellWidth)
                {
                    for (float x = -worldRadius; x < worldRadius; x += cellWidth)
                    {
                        Cell cell = new Cell();
                        cell.bounds.min = new Vector3(x, y, z);
                        cell.bounds.max = new Vector3(x + cellWidth, y, z + cellWidth);
                        cell.number = num++;
                        cells.Add(cell);
                    }
                }
            }
            spaceWidth = (int) ((worldRadius * 2.0f) / cellWidth);

            //Now find each of the neighbours for each cell
            foreach (Cell cell in cells)
            {
            	Vector3 extra = new Vector3(10, 0, 10);
                Bounds expanded = cell.bounds;
                expanded.min = expanded.min - extra;
                expanded.max = expanded.max + extra;
                foreach (Cell neighbour in cells)
                {
                    if (cell != neighbour)
                    {
                        if (neighbour.Intersects(expanded))
                        {
                            cell.adjacent.Add(neighbour);
                        }
                    }
                }
            }
        }

        public int FindCell(Vector3 pos)
        {          

            pos.y = 0;            
			pos.x += worldRadius;
			pos.z += worldRadius;
			int cellNumber = ((int)(pos.x / cellWidth))
				+ ((int)(pos.z / cellWidth)) * spaceWidth;

            if ((cellNumber >= cells.Count) || (cellNumber < 0))
            {
                return -1;
            }
            else
            {
                return cellNumber;
            }
        }

        public void Draw()
        {
            foreach (Cell cell in cells)
            {
                Vector3 tl = new Vector3(cell.bounds.min.x, cell.bounds.min.y, cell.bounds.max.z);
                Vector3 br = new Vector3(cell.bounds.max.x, cell.bounds.min.y, cell.bounds.min.z);
                LineDrawer.DrawLine(cell.bounds.min, tl, Color.cyan);
                LineDrawer.DrawLine(tl, cell.bounds.max, Color.cyan);
                LineDrawer.DrawLine(cell.bounds.max, br, Color.cyan);
                LineDrawer.DrawLine(br, cell.bounds.min, Color.cyan);
            }
        }

        public void Partition()
        {
            if (boids == null)
            {
                boids = GameObject.FindGameObjectsWithTag("boid");            
            }
            if (!needsPartitioning)
            {
                return;
            }
            foreach (Cell cell in cells)
            {
                cell.contained.Clear();
            }
            foreach (GameObject boid in boids)
            {
                int cell = FindCell(boid.transform.position);
                if (cell != -1)
                {
                    cells[cell].contained.Add(boid);
               }
            }
        }
    }
}
