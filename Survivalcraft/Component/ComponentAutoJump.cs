using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentAutoJump : Component, IUpdateable
	{
		public SubsystemTime m_subsystemTime;

		public SubsystemTerrain m_subsystemTerrain;

		public ComponentCreature m_componentCreature;

		public double m_lastAutoJumpTime;

		public bool m_alwaysEnabled;

		public float m_jumpStrength;

		public bool m_collidedWithBody;

		public UpdateOrder UpdateOrder => UpdateOrder.Default;

		public void Update(float dt)
		{
			if ((SettingsManager.AutoJump || m_alwaysEnabled) && m_subsystemTime.GameTime - m_lastAutoJumpTime > 0.25)
			{
				Vector2? lastWalkOrder = m_componentCreature.ComponentLocomotion.LastWalkOrder;
				if (lastWalkOrder.HasValue)
				{
					var vector = new Vector2(m_componentCreature.ComponentBody.CollisionVelocityChange.X, m_componentCreature.ComponentBody.CollisionVelocityChange.Z);
					if (vector != Vector2.Zero && !m_collidedWithBody)
					{
						var v = Vector2.Normalize(vector);
						Vector3 vector2 = (m_componentCreature.ComponentBody.Matrix.Right * lastWalkOrder.Value.X) + (m_componentCreature.ComponentBody.Matrix.Forward * lastWalkOrder.Value.Y);
						var v2 = Vector2.Normalize(new Vector2(vector2.X, vector2.Z));
						bool flag = false;
						Vector3 v3 = Vector3.Zero;
						Vector3 vector3 = Vector3.Zero;
						Vector3 vector4 = Vector3.Zero;
						if (Vector2.Dot(v2, -v) > 0.6f)
						{
							if (Vector2.Dot(v2, Vector2.UnitX) > 0.6f)
							{
								v3 = m_componentCreature.ComponentBody.Position + Vector3.UnitX;
								vector3 = v3 - Vector3.UnitZ;
								vector4 = v3 + Vector3.UnitZ;
								flag = true;
							}
							else if (Vector2.Dot(v2, -Vector2.UnitX) > 0.6f)
							{
								v3 = m_componentCreature.ComponentBody.Position - Vector3.UnitX;
								vector3 = v3 - Vector3.UnitZ;
								vector4 = v3 + Vector3.UnitZ;
								flag = true;
							}
							else if (Vector2.Dot(v2, Vector2.UnitY) > 0.6f)
							{
								v3 = m_componentCreature.ComponentBody.Position + Vector3.UnitZ;
								vector3 = v3 - Vector3.UnitX;
								vector4 = v3 + Vector3.UnitX;
								flag = true;
							}
							else if (Vector2.Dot(v2, -Vector2.UnitY) > 0.6f)
							{
								v3 = m_componentCreature.ComponentBody.Position - Vector3.UnitZ;
								vector3 = v3 - Vector3.UnitX;
								vector4 = v3 + Vector3.UnitX;
								flag = true;
							}
						}
						if (flag)
						{
							int cellValue = m_subsystemTerrain.Terrain.GetCellValue(Terrain.ToCell(v3.X), Terrain.ToCell(v3.Y), Terrain.ToCell(v3.Z));
							int cellValue2 = m_subsystemTerrain.Terrain.GetCellValue(Terrain.ToCell(vector3.X), Terrain.ToCell(vector3.Y), Terrain.ToCell(vector3.Z));
							int cellValue3 = m_subsystemTerrain.Terrain.GetCellValue(Terrain.ToCell(vector4.X), Terrain.ToCell(vector4.Y), Terrain.ToCell(vector4.Z));
							int cellValue4 = m_subsystemTerrain.Terrain.GetCellValue(Terrain.ToCell(v3.X), Terrain.ToCell(v3.Y) + 1, Terrain.ToCell(v3.Z));
							int cellValue5 = m_subsystemTerrain.Terrain.GetCellValue(Terrain.ToCell(vector3.X), Terrain.ToCell(vector3.Y) + 1, Terrain.ToCell(vector3.Z));
							int cellValue6 = m_subsystemTerrain.Terrain.GetCellValue(Terrain.ToCell(vector4.X), Terrain.ToCell(vector4.Y) + 1, Terrain.ToCell(vector4.Z));
							int cellContents = Terrain.ExtractContents(cellValue);
							int cellContents2 = Terrain.ExtractContents(cellValue2);
							int cellContents3 = Terrain.ExtractContents(cellValue3);
							int cellContents4 = Terrain.ExtractContents(cellValue4);
							int cellContents5 = Terrain.ExtractContents(cellValue5);
							int cellContents6 = Terrain.ExtractContents(cellValue6);
							Block block = BlocksManager.Blocks[cellContents];
							Block block2 = BlocksManager.Blocks[cellContents2];
							Block block3 = BlocksManager.Blocks[cellContents3];
							Block block4 = BlocksManager.Blocks[cellContents4];
							Block block5 = BlocksManager.Blocks[cellContents5];
							Block block6 = BlocksManager.Blocks[cellContents6];
							if (!block.NoAutoJump && ((block.IsCollidable_(cellValue) && !block4.IsCollidable_(cellValue4)) || (block2.IsCollidable_(cellValue2) && !block5.IsCollidable_(cellValue5)) || (block3.IsCollidable_(cellValue3) && !block6.IsCollidable_(cellValue6))))
							{
								m_componentCreature.ComponentLocomotion.JumpOrder = MathUtils.Max(m_jumpStrength, m_componentCreature.ComponentLocomotion.JumpOrder);
								m_lastAutoJumpTime = m_subsystemTime.GameTime;
							}
						}
					}
				}
			}
			m_collidedWithBody = false;
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			m_subsystemTerrain = Project.FindSubsystem<SubsystemTerrain>(throwOnError: true);
			m_subsystemTime = Project.FindSubsystem<SubsystemTime>(throwOnError: true);
			m_componentCreature = Entity.FindComponent<ComponentCreature>(throwOnError: true);
			m_alwaysEnabled = valuesDictionary.GetValue<bool>("AlwaysEnabled");
			m_jumpStrength = valuesDictionary.GetValue<float>("JumpStrength");
			m_componentCreature.ComponentBody.CollidedWithBody += delegate
			{
				m_collidedWithBody = true;
			};
		}
	}
}
