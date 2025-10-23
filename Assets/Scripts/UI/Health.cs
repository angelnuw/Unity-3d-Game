using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int max = 100;
    public int Max => max;

    public int Current { get; private set; }

    // UI/HUD & listeners
    public event Action<int, int> OnChanged;     // (current, max)
    public event Action<int> OnDamaged;     // amount
    public event Action<int> OnHealed;      // amount
    public event Action OnDied;

    public bool IsDead => Current <= 0;

    void Awake()
    {
        Current = Mathf.Clamp(Current <= 0 ? max : Current, 0, max);
        OnChanged?.Invoke(Current, Max);
    }

    public void SetMax(int newMax, bool clampToNewMax = true)
    {
        max = Mathf.Max(1, newMax);
        if (clampToNewMax) Current = Mathf.Min(Current, max);
        OnChanged?.Invoke(Current, max);
    }

    public void Damage(int amount)
    {
        if (amount <= 0 || Current <= 0) return;

        Current = Mathf.Max(0, Current - amount);
        OnDamaged?.Invoke(amount);
        OnChanged?.Invoke(Current, Max);

        if (Current == 0)
            OnDied?.Invoke();
    }

    public void Heal(int amount)
    {
        if (amount <= 0 || Current <= 0) return;

        int before = Current;
        Current = Mathf.Min(Max, Current + amount);
        int healed = Current - before;

        if (healed > 0) OnHealed?.Invoke(healed);
        OnChanged?.Invoke(Current, Max);
    }

    public void Refill()
    {
        Current = Max;
        OnChanged?.Invoke(Current, Max);
    }

    public void ReviveFull()
    {
        Current = Max;
        OnChanged?.Invoke(Current, Max);
    }
}
