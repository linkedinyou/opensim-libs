using System;

namespace Rednettle.Warp3D
{
	/// <summary>
	/// Summary description for warp_Camera.
	/// </summary>
	/// 
	public class warp_Camera
	{
		public warp_Matrix matrix=new warp_Matrix();
		public warp_Matrix normalmatrix=new warp_Matrix();

		bool needsRebuild=true;   // Flag indicating changes on matrix

		// Camera settings
		public warp_Vector pos=new warp_Vector(0f,0f,0f);
		public warp_Vector lookat=new warp_Vector(0f,0f,0f);
		public float rollfactor=0f;

		public float fovfact;             // Field of View factor
		public int screenwidth;
		public int screenheight;
		public int screenscale;

		public warp_Camera()
		{
			setFov(90f);
		}

		public warp_Camera(float fov)
		{
			setFov(fov);
		}

		public warp_Matrix getMatrix()
		{
			rebuildMatrices();
			return matrix;
		}

		public warp_Matrix getNormalMatrix()
		{
			rebuildMatrices();
			return normalmatrix;
		}

		void rebuildMatrices()
		{
			if (!needsRebuild) return;
			needsRebuild=false;

			warp_Vector forward,up,right;

			forward=warp_Vector.sub(lookat,pos);
			up=new warp_Vector(0f,1f,0f);
			right=warp_Vector.getNormal(up,forward);
			up=warp_Vector.getNormal(forward,right);

			forward.normalize();
			up.normalize();
			right.normalize();

			normalmatrix=new warp_Matrix(right,up,forward);
			normalmatrix.rotate(0,0,rollfactor);
			matrix=normalmatrix.getClone();
			matrix.shift(pos.x,pos.y,pos.z);

			normalmatrix=normalmatrix.inverse();
			matrix=matrix.inverse();
		}

		public void setFov(float fov)
		{
			fovfact=(float)Math.Tan(warp_Math.deg2rad(fov)/2);
		}

		public void roll(float angle)
		{
			rollfactor+=angle;
			needsRebuild=true;
		}

		public void setPos(float px, float py, float pz)
		{
			pos=new warp_Vector(px,py,pz);
			needsRebuild=true;
		}

		public void setPos(warp_Vector p)
		{
			pos=p;
			needsRebuild=true;
		}

		public void lookAt(float px, float py, float pz)
		{
			lookat=new warp_Vector(px,py,pz);
			needsRebuild=true;
		}

		public void lookAt(warp_Vector p)
		{
			lookat=p;
			needsRebuild=true;
		}

		public void setScreensize(int w, int h)
		{
			screenwidth=w;
			screenheight=h;
			screenscale=(w<h)?w:h;
		}

		public void shift(float dx, float dy, float dz)
		{
			pos=pos.transform(warp_Matrix.shiftMatrix(dx,dy,dz));
			lookat=lookat.transform(warp_Matrix.shiftMatrix(dx,dy,dz));
			needsRebuild=true;
		}

		public void shift(warp_Vector v)
		{
			shift(v.x,v.y,v.z);
		}

		public void rotate(float dx, float dy, float dz)
		{
			pos=pos.transform(warp_Matrix.rotateMatrix(dx,dy,dz));
			needsRebuild=true;
		}

		public void rotate(warp_Vector v)
		{
			rotate(v.x,v.y,v.z);
		}

		public static warp_Camera FRONT()
		{
			warp_Camera cam=new warp_Camera();
			cam.setPos(0,0,-2f);
			return cam;
		}

		public static warp_Camera LEFT()
		{
			warp_Camera cam=new warp_Camera();
			cam.setPos(2f,0,0);
			return cam;
		}

		public static warp_Camera RIGHT()
		{
			warp_Camera cam=new warp_Camera();
			cam.setPos(-2f,0,0);
			return cam;
		}

		public static warp_Camera TOP()
		{
			warp_Camera cam=new warp_Camera();
			cam.setPos(0,-2f,0);
			return cam;
		}
	}
}
