//lettercan.cs
//spray can shoots projectiles that change the color of bricks

//sound
datablock AudioProfile(sprayFireSound)
{
   filename    = "~/data/sound/sprayLoop.wav";
   description = AudioClosestLooping3d;
   preload = true;
};

datablock AudioProfile(sprayHitSound)
{
   filename    = "~/data/sound/sprayHit.wav";
   description = AudioClosest3d;
   preload = true;
};
datablock AudioProfile(sprayActivateSound)
{
   filename    = "~/data/sound/sprayActivate.wav";
   description = AudioClosest3d;
   preload = true;
};

datablock ProjectileData(lettercanProjectile)
{
   //projectileShapeName = "~/data/shapes/arrow.dts";
   //explosion           = lettercanExplosion;
   //particleEmitter     = lettercanTrailEmitter;

   muzzleVelocity      = 300;
   velInheritFactor    = 0;

   armingDelay         = 0;
   lifetime            = 100;
   fadeDelay           = 0;
   bounceElasticity    = 0;
   bounceFriction      = 0;
   isBallistic         = false;
   gravityMod = 0.0;

   hasLight    = false;
   lightRadius = 3.0;
   lightColor  = "0 0 0.5";
};

/////////
//ITEMS//
/////////
datablock ItemData(lettercan)
{
	category = "Tools";  // Mission editor category
	className = "tool";   // For inventory system

	 // Basic Item Properties
	shapeFile = "~/data/shapes/sprayCan.dts";
	skinName = 'white';
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = false;
	cost = 30;

	 // Dynamic properties defined by the scripts
	pickUpName = 'a letter can.';
	invName = 'Letter Can';
	image = lettercanImage;
};



//////////
//IMAGES//
//////////
datablock ShapeBaseImageData(lettercanImage)
{
   // Basic Item properties
   shapeFile = "~/data/shapes/spraycan.dts";
   skinName = 'white';
   emap = false;
	cloakable = false;
   mountPoint = 0;
   offset = "0 0 0";

   correctMuzzleVector = true;

   // Add the WeaponImage namespace as a parent, WeaponImage namespace
   // provides some hooks into the inventory system.
   className = "WeaponImage";

   // Projectile && Ammo.
   item = lettercan;
   ammo = " ";
   projectile = lettercanProjectile;
   projectileType = Projectile;

   //melee particles shoot from eye node for consistancy
   melee = false;
   //raise your arm up or not
   armReady = true;

   //casing = " ";

   // Images have a state system which controls how the animations
   // are run, which sounds are played, script callbacks, etc. This
   // state system is downloaded to the client so that clients can
   // predict state changes and animate accordingly.  The following
   // system supports basic ready->fire->reload transitions as
   // well as a no-ammo->dryfire idle state.

   // Initial start up state
	stateName[0]                     = "Activate";
	stateTimeoutValue[0]             = 0.0;
	stateTransitionOnTimeout[0]       = "Ready";
	stateSound[0]					= sprayActivateSound;

	stateName[1]                    = "Ready";
	stateTransitionOnTriggerDown[1] = "Fire";
	stateAllowImageChange[1]        = true;
	

	stateName[2]					= "Fire";
	stateScript[2]                  = "onFire";
	stateFire[2]					= true;
	stateAllowImageChange[2]        = true;
	stateTimeoutValue[2]            = 0.10;
	stateTransitionOnTimeout[2]     = "Fire";
	stateTransitionOnTriggerUp[2]	= "Ready";
	//stateEmitter[2]					= lettercanExplosionEmitter;
	//stateEmitterTime[2]				= 0.1;
	stateSound[2]					= sprayFireSound;

};

///////////
//METHODS//
///////////
//using the paint can item
function lettercan::onUse(%this,%player, %invPosition)
{
	%client = %player.client;
	if(%client)
	{
		%color = %client.color;
	}
	%mountedImage = %player.getMountedImage($RightHandSlot);

	//if a spray can is already mounted
	if(%mountedImage.item $= "lettercan" && (%player.currWeaponSlot == %invPosition)) 
	{
		%client.letterIndex++;

		if(%client.letterIndex > $TotalLetters)
		{
			%client.letterIndex = 0;
		}	
		commandtoclient(%client,'ShowBrickImage',$LetterPreview[%client.letterIndex]);
	}
	else 
	{
		//if the client has a color, use that
		%image = nameToID("lettercanImage");
		%player.mountImage(%image, $RightHandSlot, 1, %image.skinName);
		//hilight inv slot
		messageClient(%client, 'MsgHilightInv', '', %InvPosition);
		%player.currWeaponSlot = %invPosition;
		%client.color = $Letter[%obj.client.letterIndex];
		commandtoclient(%client,'ShowBrickImage',$LetterPreview[%client.letterIndex]);
	}
}

//Collisions//

function lettercanProjectile::onCollision(%this,%obj,%col,%fade,%pos,%normal)
{
	%isTrusted = checkSafe(%col,%obj.client);
	
if(%obj.client.isImprisoned)
	{
		messageClient(%client,'',"\c4No painting you imprisoned, no good, peice of poo!");
		return;
	}

	if (%col.getClassName() $= "Player")
			{
			if (%col.client.isAdmin || %col.client.isSuperAdmin)
			{
			messageClient(%obj.client,'','\c2You can\'t paint Admins!');
			return;
			}
			}

	if(%col.owner == %obj.client || (%col.owner.secure == 0 && %col.OwnerAway == 0)|| %isTrusted || %obj.client.isAdmin || %obj.client.isSuperAdmin)
	{
		if(%col.getClassName() $= "StaticShape" || %col.getClassName() $= "Player")
		{
			if(%col.getSkinName() !$= $Letter[%obj.client.letterIndex])
			{	
				%obj.client.Undo[0] = 1;
				%obj.client.Undo[1] = %col;
				%obj.client.Undo[2] = %col.getSkinName();
				%col.setSkinName($Letter[%obj.client.letterIndex]);		
				%col.SkinName = %col.getSkinName();
			}
		}
	}
}

function checkSafe(%col,%client)
{
	%isTrusted = 0;

	for(%trusted = 0; %trusted < clientGroup.GetCount(); %trusted++)
	{
		//echo(clientGroup.GetCount());
		%cl = clientGroup.getObject(%trusted);
		//echo(%cl);
		for(%safe = 0; %safe < %col.Owner.SafeListNum + 1; %safe++)
		{
			//echo(%col.Owner.SafeList[%safe]);
			if(%col.Owner.SafeList[%safe] == %cl && %cl == %client)
			{
				%isTrusted = 1;
			}
		}

	} 
	return %isTrusted;
}