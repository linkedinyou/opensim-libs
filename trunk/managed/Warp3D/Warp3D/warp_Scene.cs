using System;
using System.Collections;
using System.Drawing;
using System.Timers;

namespace Rednettle.Warp3D
{
	/// <summary>
	/// Summary description for warp_Scene.
	/// </summary>
	public class warp_Scene : warp_CoreObject
	{
		public static String version="1.0.0";
		public static String release="0010";

		public warp_RenderPipeline renderPipeline;
		public int width,height;

		public warp_Environment environment=new warp_Environment();
		public warp_Camera defaultCamera=warp_Camera.FRONT();

		public warp_Object[] wobject;
		public warp_Light[] light;

		public int objects=0;
		public int lights=0;

		private bool objectsNeedRebuild=true;
		private bool lightsNeedRebuild=true;

		protected bool preparedForRendering=false;

		public warp_Vector normalizedOffset=new warp_Vector(0f,0f,0f);
		public float normalizedScale=1f;
		private static bool instancesRunning=false;

		public Hashtable objectData=new Hashtable();
		public Hashtable lightData=new Hashtable();
		public Hashtable materialData=new Hashtable();
		public Hashtable cameraData=new Hashtable();

		int probes = 0;
		int perf = 0;

		public String fps = "0";

		public int rendertime = 500;

		public warp_Scene()
		{
		}

		public warp_Scene(int w, int h)
		{
			showInfo();
			width=w;
			height=h;
			renderPipeline = new warp_RenderPipeline(this,w,h);
		}

		public void showInfo()
		{
			if (instancesRunning) return;

			System.Console.WriteLine();
			System.Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
			System.Console.WriteLine("WARP 3D Kernel "+version+" [Build "+release+"]");
			System.Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

			instancesRunning=true;
		}

		public void removeAllObjects()
		{
			objectData=new Hashtable();
			objectsNeedRebuild=true;
			rebuild();
		}

		public void rebuild()
		{
			if (objectsNeedRebuild)
			{
				objectsNeedRebuild=false;

				objects=objectData.Count;
				wobject=new warp_Object[objects];
				IDictionaryEnumerator enumerator = objectData.GetEnumerator();

				for (int i=objects-1;i>=0;i--)
				{
					enumerator.MoveNext();
					wobject[i]=(warp_Object)enumerator.Value;

					wobject[i].id=i;
					wobject[i].rebuild();
				}
			}

			if (lightsNeedRebuild)
			{
				lightsNeedRebuild = false;
				lights = lightData.Count;
				light = new warp_Light[lights];
				IDictionaryEnumerator enumerator = lightData.GetEnumerator();
				for (int i = lights - 1; i >= 0; i--)
				{
					enumerator.MoveNext();
					light[i] = (warp_Light) enumerator.Value;

				}
			}
		}

		public warp_Object sceneobject(String key)
		{
			return (warp_Object) objectData[key];
		}

		public warp_Material material(String key)
		{
			return (warp_Material) materialData[key];
		}

		public warp_Camera camera(String key)
		{
			return (warp_Camera) cameraData[key];
		}

		public void addObject(String key, warp_Object obj)
		{
			obj.name=key;
			objectData.Add(key, obj);
			obj.parent=this;
			objectsNeedRebuild=true;
		}

		public void removeObject(String key)
		{
			objectData.Remove(key);
			objectsNeedRebuild=true;
			preparedForRendering=false;
		}

		public void addMaterial(String key, warp_Material m)
		{
			materialData.Add(key, m);
		}

		public void removeMaterial(String key)
		{
			materialData.Remove(key);
		}

		public void addCamera(String key, warp_Camera c)
		{
			cameraData.Add(key, c);
		}

		public void removeCamera(String key)
		{
			cameraData.Remove(key);
		}

		public void addLight(String key, warp_Light l)
		{
			lightData.Add(key, l);
			lightsNeedRebuild = true;
		}

		public void removeLight(String key)
		{
			lightData.Remove(key);
			lightsNeedRebuild = true;
			preparedForRendering = false;
		}

		public void prepareForRendering()
		{
			if (preparedForRendering) return;
			preparedForRendering=true;

			System.Console.WriteLine("warp_Scene| prepareForRendering : Preparing for realtime rendering ...");
			rebuild();
			renderPipeline.buildLightMap();
			printSceneInfo();
		}

		public void printSceneInfo()
		{
			System.Console.WriteLine("warp_Scene| Objects   : "+objects);
			System.Console.WriteLine("warp_Scene| Vertices  : "+countVertices());
			System.Console.WriteLine("warp_Scene| Triangles : "+countTriangles());
		}

		public void render()
		{
			DateTime n = DateTime.Now;
			renderPipeline.render(this.defaultCamera);

			TimeSpan s  = DateTime.Now-n;

			perf+=s.Milliseconds;

			if(probes++==32)
			{
				probes=0;
				fps = (float)perf/32+","+(float)1000/ (float)(perf/32);

				perf=0;
			}

		}

		public Bitmap getImage()
		{
			return renderPipeline.screen.getImage();
		}

		public String getFPS()
		{
			return fps;
		}

		/*
		public java.awt.Dimension size()
		{
			return new java.awt.Dimension(width,height);
		}
		*/

		public void setBackgroundColor(int bgcolor)
		{
			environment.bgcolor = bgcolor;
		}

		public void setBackground(warp_Texture t)
		{
			environment.setBackground(t);
		}

		public void setAmbient(int ambientcolor)
		{
			environment.ambient=ambientcolor;
		}

		public int countVertices()
		{
			int counter=0;
			for (int i=0;i<objects;i++) counter+=wobject[i].vertices;
			return counter;
		}

		public int countTriangles()
		{
			int counter=0;
			for (int i=0;i<objects;i++) counter+=wobject[i].triangles;
			return counter;
		}

		public void useIdBuffer(bool buffer)
		{
			renderPipeline.useIdBuffer(buffer);
		}

		public warp_Triangle identifyTriangleAt(int xpos, int ypos)
		{
			if (!renderPipeline.useId)
			{
				return null;
			}
			if (xpos < 0 || xpos >= width)
			{
				return null;
			}
			if (ypos < 0 || ypos >= height)
			{
				return null;
			}

			int pos = xpos + renderPipeline.screen.width * ypos;
			int idCode = renderPipeline.idBuffer[pos];
			if (idCode < 0)
			{
				return null;
			}
			return wobject[idCode >> 16].fasttriangle[idCode & 0xFFFF];
		}

		public warp_Object identifyObjectAt(int xpos, int ypos)
		{
			warp_Triangle tri = identifyTriangleAt(xpos, ypos);
			if (tri == null)
			{
				return null;
			}
			return tri.parent;
		}

		public void normalize()
		{
			objectsNeedRebuild = true;
			rebuild();

			warp_Vector min, max, tempmax, tempmin;
			if (objects == 0)
			{
				return;
			}

			matrix = new warp_Matrix();
			normalmatrix = new warp_Matrix();

			max = wobject[0].maximum();
			min = wobject[0].maximum();

			for (int i = 0; i < objects; i++)
			{
				tempmax = wobject[i].maximum();
				tempmin = wobject[i].maximum();
				if (tempmax.x > max.x)
				{
					max.x = tempmax.x;
				}
				if (tempmax.y > max.y)
				{
					max.y = tempmax.y;
				}
				if (tempmax.z > max.z)
				{
					max.z = tempmax.z;
				}
				if (tempmin.x < min.x)
				{
					min.x = tempmin.x;
				}
				if (tempmin.y < min.y)
				{
					min.y = tempmin.y;
				}
				if (tempmin.z < min.z)
				{
					min.z = tempmin.z;
				}
			}
			float xdist = max.x - min.x;
			float ydist = max.y - min.y;
			float zdist = max.z - min.z;
			float xmed = (max.x + min.x) / 2;
			float ymed = (max.y + min.y) / 2;
			float zmed = (max.z + min.z) / 2;

			float diameter = (xdist > ydist) ? xdist : ydist;
			diameter = (zdist > diameter) ? zdist : diameter;

			normalizedOffset = new warp_Vector(xmed, ymed, zmed);
			normalizedScale = 2 / diameter;

			shift(normalizedOffset.reverse());
			scale(normalizedScale);
		}
	}
}
