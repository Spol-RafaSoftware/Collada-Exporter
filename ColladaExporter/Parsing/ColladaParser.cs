using System;
using System.Xml;
using System.IO;

namespace ColladaExporter.Parsing
{
	public class ColladaParser : Parser
	{
		public const String LIBRARY_GEOMETRIES = "library_geometries";
		
		public ColladaParser(String path)
		{	
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(path);
			
			XmlElement RootElement = xmlDoc.DocumentElement;
			
			for (int i = 0; i < RootElement.ChildNodes.Count; i++)
			{
				XmlNode ChildNode = RootElement.ChildNodes[i];
				String ChildName = ChildNode.Name.ToString();
				
				if (ChildName.Equals(LIBRARY_GEOMETRIES))
				{
					ParseGeometryLibrary(ChildNode);
				}
			}
			
			isFinished = true;
		}
		
		public void ParseGeometryLibrary(XmlNode GeomNode)
		{
			Console.WriteLine("Entered GeometryLibraryNode");
			
			for (int i = 0; i < GeomNode.ChildNodes.Count; i++) {
				
				if (GeomNode.ChildNodes[i].Name.Equals("geometry"))
				{
					ParseGeometryLibrary_Geometry(GeomNode.ChildNodes[i]);
				}
			}
		}
		
		public void ParseGeometryLibrary_Geometry(XmlNode GeomNode)
		{
			Console.WriteLine("Entered GeometryNode");
			
			Geometry CurrentGeometry = new Geometry();
			
			for (int i = 0; i < GeomNode.ChildNodes.Count; i++)
			{
				if (GeomNode.ChildNodes[i].Name.Equals("mesh"))
				{
					ParseGeometryLibrary_GeometryMesh(GeomNode.ChildNodes[i], CurrentGeometry);
				}
			}
			
			mGeometryLibrary.mGeometries.Add(CurrentGeometry);
		}
		
		public void ParseGeometryLibrary_GeometryMesh(XmlNode MeshNode, Geometry CurrentGeometry)
		{
			Console.WriteLine("Entered MeshNode");
			
			Mesh CurrentMesh = new Mesh();
			
			for (int i = 0; i < MeshNode.ChildNodes.Count; i++)
			{
				if (MeshNode.ChildNodes[i].Name.Equals("source"))
				{
					ParseGeometryLibrary_GeometryMeshSource(MeshNode.ChildNodes[i], CurrentMesh);
				}
				
				if (MeshNode.ChildNodes[i].Name.Equals("vertices"))
				{
					ParseGeometryLibrary_GeometryMeshVertices(MeshNode.ChildNodes[i], CurrentMesh);
				}
				
				if (MeshNode.ChildNodes[i].Name.Equals("triangles"))
				{
					ParseGeometryLibrary_GeometryMeshTriangles(MeshNode.ChildNodes[i], CurrentMesh);
				}
			}
			
			CurrentGeometry.mMeshes.Add(CurrentMesh);
		}
		
		public void ParseGeometryLibrary_GeometryMeshSource(XmlNode SourceNode, Mesh CurrentMesh)
		{
			Console.WriteLine("Entered SourceNode");
			
			Source CurrentSource = new Source();
			
			for (int i = 0; i < SourceNode.Attributes.Count; i++)
			{
				switch(SourceNode.Attributes[i].Name)
				{
					case "id":
						CurrentSource.mSourceId = SourceNode.Attributes[i].Value.ToString();
						break;
				}
			}
			
			for (int i = 0; i < SourceNode.ChildNodes.Count; i++)
			{
				if (SourceNode.ChildNodes[i].Name.Equals("float_array"))
				{
					ParseGeometryLibrary_GeometryMeshSource_FloatArray(SourceNode.ChildNodes[i], CurrentSource);
				}
				
				if (SourceNode.ChildNodes[i].Name.Equals("technique_common"))
				{
					ParseGeometryLibrary_GeometryMeshSource_TechniqueCommon(SourceNode.ChildNodes[i], CurrentSource);
				}
			}
			
			CurrentMesh.mSources.Add(CurrentSource);
		}
		
		public void ParseGeometryLibrary_GeometryMeshVertices(XmlNode VerticeNode, Mesh CurrentMesh)
		{
			Console.WriteLine("Entered VerticesNode");
			
			Vertices CurrentVertices = new Vertices();
			
			for (int i = 0; i < VerticeNode.ChildNodes.Count; i++)
			{
				for (int j = 0; j < VerticeNode.Attributes.Count; j++)
				{
					if (VerticeNode.Attributes[j].Name.Equals("id"))
					{
						CurrentVertices.mVerticesId = VerticeNode.Attributes[j].Value.ToString();
					}
				}
				
				
				if (VerticeNode.ChildNodes[i].Name.Equals("input"))
				{
					Console.WriteLine("Entered InputNode");
					XmlNode InputNode = VerticeNode.ChildNodes[i];
					
					for (int j = 0; j < InputNode.Attributes.Count; j++)
					{
						switch(InputNode.Attributes[j].Name)
						{
							case "semantic":
								CurrentVertices.mInputSemantic = InputNode.Attributes[j].Value.ToString();
								break;
							case "source":
								CurrentVertices.mInputSource = InputNode.Attributes[j].Value.ToString().Substring(1);
								break;
						}
					}
				}
			}
			
			CurrentMesh.mVertices = CurrentVertices;
		}
		
		public void ParseGeometryLibrary_GeometryMeshTriangles(XmlNode TrianglesNode, Mesh CurrentMesh)
		{
			Console.WriteLine("Entered TrianglesNode");
			
			Triangles CurrentTriangles = new Triangles();
			
			for (int i = 0; i < TrianglesNode.Attributes.Count; i++)
			{
				switch(TrianglesNode.Attributes[i].Name)
				{
					case "count":
						CurrentTriangles.mCount = uint.Parse(TrianglesNode.Attributes[i].Value);
						break;
					case "material":
						CurrentTriangles.mMaterial = TrianglesNode.Attributes[i].Value.ToString();
						break;
				}
			}
			
			for (int i = 0; i < TrianglesNode.ChildNodes.Count; i++)
			{
				if (TrianglesNode.ChildNodes[i].Name.Equals("input"))
				{
					ParseGeometryLibrary_GeometryMeshTriangles_Input(TrianglesNode.ChildNodes[i], CurrentTriangles);
				}
				
				if (TrianglesNode.ChildNodes[i].Name.Equals("p"))
				{
					ParseGeometryLibrary_GeometryMeshTriangles_P(TrianglesNode.ChildNodes[i], CurrentTriangles);
				}
			}
			
			CurrentMesh.mTriangles.Add(CurrentTriangles);
		}
		
		public void ParseGeometryLibrary_GeometryMeshSource_FloatArray(XmlNode FloatArrayNode, Source CurrentSource)
		{
			Console.WriteLine("Entered FloatArrayNode");
			
			FloatArray CurrentFloatArray = new FloatArray();
			
			for (int i = 0; i < FloatArrayNode.Attributes.Count; i++)
			{
				switch(FloatArrayNode.Attributes[i].Name)
				{
					case "count":
						CurrentFloatArray.mCount = uint.Parse(FloatArrayNode.Attributes[i].Value);
						break;
					case "id":
						CurrentFloatArray.mFloatArrayId = FloatArrayNode.Attributes[i].Value.ToString();
						break;
				}
			}
			
			if (CurrentFloatArray.mCount != 0)
			{
				String floats = FloatArrayNode.InnerText;
				String delimter = " ";
				String[] floatValues = floats.Split(delimter.ToCharArray());
				
				for (int i = 0; i < floatValues.Length; i++)
				{
					CurrentFloatArray.mFloats.Add(Convert.ToSingle(floatValues[i], System.Globalization.CultureInfo.InvariantCulture));
				}	
			}
			
			CurrentSource.mFloatArray = CurrentFloatArray;
		}
		
		public void ParseGeometryLibrary_GeometryMeshSource_TechniqueCommon(XmlNode TechniqueCommonNode, Source CurrentSource)
		{
			Console.WriteLine("Entered TechniqueCommonNode");
			
			TechniqueCommon CurrentTechniqueCommon = new TechniqueCommon();
			
			for (int i = 0; i < TechniqueCommonNode.ChildNodes.Count; i++)
			{
				if (TechniqueCommonNode.ChildNodes[i].Name.Equals("accessor"))
				{
					Console.WriteLine("Entered AccessorNode");
					
					XmlNode AccessorNode = TechniqueCommonNode.ChildNodes[i];
					Accessor CurrentAccessor = new Accessor();
					
					for (int j = 0; j < AccessorNode.Attributes.Count; j++)
					{
						switch(AccessorNode.Attributes[i].Name)
						{
							case "count":
								CurrentAccessor.mCount = uint.Parse(AccessorNode.Attributes[i].Value);
								break;
							case "stride":
								CurrentAccessor.mStride = uint.Parse(AccessorNode.Attributes[i].Value);
								break;
							case "source":
								CurrentAccessor.mSource = AccessorNode.Attributes[i].Value.ToString().Substring(1);
								break;
						}
					}
					
					for (int j = 0; j < AccessorNode.ChildNodes.Count; j++)
					{
						if (AccessorNode.ChildNodes[i].Name.Equals("param"))
						{
							Console.WriteLine("Entered ParamNode");
							XmlNode ParamNode = AccessorNode.ChildNodes[i];
							Param CurrentParam = new Param();
							
							for (int k = 0; k < ParamNode.Attributes.Count; k++)
							{
								switch(ParamNode.Attributes[i].Name)
								{
									case "name":
										CurrentParam.mName =  ParamNode.Attributes[i].Value.ToString();
										break;
									case "type":
										CurrentParam.mType = ParamNode.Attributes[i].Value.ToString();
										break;
								}
							}
							
							CurrentAccessor.mParams.Add(CurrentParam);
						}
					}
					
					CurrentTechniqueCommon.mAccessor = CurrentAccessor;
				}
			}
			
			CurrentSource.mTechniqueCommon = CurrentTechniqueCommon;
		}
		
		public void ParseGeometryLibrary_GeometryMeshTriangles_Input(XmlNode InputNode, Triangles CurrentTriangles)
		{
			Console.WriteLine("Entered TrianglesInputNode");
			TrianglesInput CurrentTrianglesInput = new TrianglesInput();
			
			for (int i = 0; i < InputNode.Attributes.Count; i++)
			{
				switch(InputNode.Attributes[i].Name)
				{
					case "offset":
						CurrentTrianglesInput.mOffset = uint.Parse(InputNode.Attributes[i].Value);
						break;
					case "semantic":
						CurrentTrianglesInput.mSemantic = InputNode.Attributes[i].Value.ToString();
						break;
					case "set":
						// Ignored
						break;
					case "source":
						CurrentTrianglesInput.mSource = InputNode.Attributes[i].Value.ToString().Substring(1);
						break;
				}
			}
			
			CurrentTriangles.mTriangleInputs.Add(CurrentTrianglesInput);
		}
		
		public void ParseGeometryLibrary_GeometryMeshTriangles_P(XmlNode PNode, Triangles CurrentTriangles)
		{
			Console.WriteLine("Entered TrianglesPNode");
			TrianglesP CurrentTrianglesP = new TrianglesP();
			
			String PValues = PNode.InnerText;
			String[] Ps = PValues.Split(" ".ToCharArray());
			
			for (int i = 0; i < Ps.Length; i++)
			{
				CurrentTrianglesP.mIndices.Add(uint.Parse(Ps[i]));
			}
			
			CurrentTriangles.mTrianglesP = CurrentTrianglesP;
		}
	}
}

