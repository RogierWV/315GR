#include "IAnimationGraph.h"

ENUM_INFO_BEGIN(EAnimationGraphInputType)
	ENUM_ELEM_INFO(, eAGIT_Integer)
	ENUM_ELEM_INFO(, eAGIT_Float)
	ENUM_ELEM_INFO(, eAGIT_String)
ENUM_INFO_END(EAnimationGraphInputType)

ENUM_INFO_BEGIN(EMovementControlMethod)
	ENUM_ELEM_INFO(, eMCM_Undefined)
	ENUM_ELEM_INFO(, eMCM_Entity)
	ENUM_ELEM_INFO(, eMCM_Animation)
	ENUM_ELEM_INFO(, eMCM_DecoupledCatchUp)
	ENUM_ELEM_INFO(, eMCM_ClampedEntity)
	ENUM_ELEM_INFO(, eMCM_SmoothedEntity)
	ENUM_ELEM_INFO(, eMCM_AnimationHCollision)
	ENUM_ELEM_INFO(, eMCM_COUNT)
	ENUM_ELEM_INFO(, eMCM_FF)
ENUM_INFO_END(EMovementControlMethod)

ENUM_INFO_BEGIN(EColliderMode)
	ENUM_ELEM_INFO(, eColliderMode_Undefined)
	ENUM_ELEM_INFO(, eColliderMode_Disabled)
	ENUM_ELEM_INFO(, eColliderMode_GroundedOnly)
	ENUM_ELEM_INFO(, eColliderMode_Pushable)
	ENUM_ELEM_INFO(, eColliderMode_NonPushable)
	ENUM_ELEM_INFO(, eColliderMode_PushesPlayersOnly)
	ENUM_ELEM_INFO(, eColliderMode_Spectator)
	ENUM_ELEM_INFO(, eColliderMode_COUNT)
	ENUM_ELEM_INFO(, eColliderMode_FF)
ENUM_INFO_END(EColliderMode)
