using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

[RequireComponent(typeof(VisualEffect))]
public class AnomalyZoneEffect : VFXOutputEventAbstractHandler
{
    [SerializeField] private bool m_IsDoT;
    [SerializeField] private float m_Damage;
    [SerializeField] private float m_Radius;
    [SerializeField] private string[] m_EffectLayerMasks;

    public override bool canExecuteInEditor => true;

    private int m_Layers = 0;

    private void Start()
    {
        m_Layers = LayerMask.GetMask(m_EffectLayerMasks);
    }

    public override void OnVFXOutputEvent(VFXEventAttribute eventAttribute)
    {
        if (m_IsDoT)
        {
            InvokeRepeating("CheckDamage", 0f, 1f);
        }
        else
        {
            CheckDamage();
        }
    }

    private void CheckDamage()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_Radius, m_Layers);
        List<int> ids = new List<int>();
        foreach (Collider collider in colliders)
        {
            if (!ids.Contains(collider.transform.root.GetInstanceID()) && collider.transform.root.TryGetComponent(out Health healthTarget))
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                float damage = m_Damage * (1 - (distance / m_Radius));

                if (damage > 1) healthTarget.Remove(damage, gameObject);
                ids.Add(collider.transform.root.GetInstanceID());
            }
        }
    }
}
