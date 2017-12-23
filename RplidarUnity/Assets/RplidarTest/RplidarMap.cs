using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

[RequireComponent(typeof(MeshFilter))]
public class RplidarMap : MonoBehaviour {

    public bool m_onscan = false;

    private LidarData[] m_data;
    public string COM = "COM3";

    public Mesh m_mesh;
    private List<Vector3> m_vert;
    private List<int> m_ind;

    private MeshFilter m_meshfilter;

    private Thread m_thread;
    private bool m_datachanged = false;
	void Start () {

        m_meshfilter = GetComponent<MeshFilter>();

        m_data = new LidarData[720];

        m_ind = new List<int>();
        m_vert = new List<Vector3>();
        for (int i = 0; i < 720; i++)
        {
            m_ind.Add(i);
        }
        m_mesh = new Mesh();
        m_mesh.MarkDynamic();

        RplidarBinding.OnConnect(COM);
        RplidarBinding.StartMotor();
        m_onscan = RplidarBinding.StartScan();


        if (m_onscan)
        {
            m_thread = new Thread(GenMesh);
            m_thread.Start();
        }

    }

    void OnDestroy()
    {
        m_thread.Abort();

        RplidarBinding.EndScan();
        RplidarBinding.EndMotor();
        RplidarBinding.OnDisconnect();
        RplidarBinding.ReleaseDrive();

        m_onscan = false;
    }

    void Update()
    {
        if (m_datachanged)
        {
            m_vert.Clear();
            for (int i = 0; i < 720; i++)
            {
                m_vert.Add(Quaternion.Euler(0, 0, m_data[i].theta) * Vector3.right * m_data[i].distant*0.01f);

            }
            m_mesh.SetVertices(m_vert);
            m_mesh.SetIndices(m_ind.ToArray(), MeshTopology.Points, 0);

            m_mesh.UploadMeshData(false);
            m_meshfilter.mesh = m_mesh;

            m_datachanged = false;
        }
    }

    void GenMesh()
    {
        while (true)
        {
            int datacount = RplidarBinding.GetData(ref m_data);
            if (datacount == 0)
            {
                Thread.Sleep(20);
            }
            else
            {
                m_datachanged = true;
            }
        }
    }

}
