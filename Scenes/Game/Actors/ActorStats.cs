using Godot;
using MonoCustomResourceRegistry;
using System;

[RegisteredTypeAttribute(nameof(ActorStats), baseType = "Resource")]
public partial class ActorStats : Resource
{
    [Export]
    public float MaxHealth = 1;

    [Export]
    public float Speed = 1;

    [Export]
    public float Damage = 1;

    [Export]
    public float Knockback = 1;

    [Export]
    public float KnockbackSpeed = 1;

    [Export]
    public float KnockbackDecceleration = 1;

    public ActorStats() : this(1, 1, 1, 1, 1, 1) { }

    public ActorStats(float maxHealth, float speed, float damage, float knockback, float knockbackSpeed, float knockbackDecceleration)
    {
        MaxHealth = maxHealth;
        Speed = speed;
        Damage = damage;
        Knockback = knockback;
        KnockbackSpeed = knockbackSpeed;
        KnockbackDecceleration = knockbackDecceleration;
    }
}
