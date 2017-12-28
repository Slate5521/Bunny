/// Bunny RTB - serverAttributes.cs
/// made by emiko/slate5521
//
// Stores attributes that can be modified in the startMissionGUI's attributes
// tab. 
//
// The "$pref::" part is removed from each value. For example, if you are
// listing $Pref::Server::Whatever, you would use Server::Whatever, because
// the $Pref part is implicit.

$serverAttribute::boolean = 0;
$serverAttribute::integer = 1;

$serverAttribute::count = -1;


// Privileges
$serverAttribute[$serverAttribute::count++] 
	=	"Item Cost Enabled" NL 
		"Server::ItemsCostMoney" NL 
		$serverAttribute::boolean;
		
$serverAttribute[$serverAttribute::count++] 
	=	"Player Starting Money" NL 
		"Server::StartMoney" NL 
		$serverAttribute::integer;
		
$serverAttribute[$serverAttribute::count++] 
	=	"Editor Wand Upon Spawn Enabled" NL 
		"Server::EWUponSpawn" NL 
		$serverAttribute::boolean;
	
$serverAttribute[$serverAttribute::count++] 
	=	"Suicide Enabled" NL 
		"Server::suicide" NL 
		$serverAttribute::boolean;
		
$serverAttribute[$serverAttribute::count++] 
	=	"Q Enabled" NL 
		"Server::freeze" NL 
		$serverAttribute::boolean;
		
$serverAttribute[$serverAttribute::count++]
	=	"R Enabled" NL 
		"Server::EnabledSit" NL 
		$serverAttribute::boolean;

// Chat Attributes
$serverAttribute[$serverAttribute::count++] //Malfunction
	=	"Chat Enabled" NL 
		"Server::PMSys" NL 
		$serverAttribute::boolean;
		
$serverAttribute[$serverAttribute::count++] //Malfunction
	=	"PM Enabled" NL 
		"Server::PMSys" NL 
		$serverAttribute::boolean;
	
$serverAttribute[$serverAttribute::count++] 
	=	"Name Monitor Enabled" NL 
		"Server::NameChangeWarning" NL 
		$serverAttribute::boolean;
		
$serverAttribute[$serverAttribute::count++] 
	=	"Imposter Warning Enabled" NL 
		"Server::ImposterWarning" NL 
		$serverAttribute::boolean;
		
$serverAttribute[$serverAttribute::count++] 
	=	"Slash Commands Enabled" NL 
		"Server::SCommands" NL 
		$serverAttribute::boolean;
		
$serverAttribute[$serverAttribute::count++] 
	=	"Flood Protection" NL 
		"Server::FloodProtectionEnabled" NL 
		$serverAttribute::boolean;
		
$serverAttribute[$serverAttribute::count++] 
	=	"Flood Protection Threshold" NL 
		"Server::FloodProtectionThreshold" NL 
		$serverAttribute::integer;
		
// Building Attributes	
$serverAttribute[$serverAttribute::count++] 
	=	"Max Player Bricks" NL 
		"Server::MaxAllowedBricks" NL 
		$serverAttribute::integer;

$serverAttribute[$serverAttribute::count++] 
	=	"Max Player Movers" NL 
		"Server::MaxAllowedMovers" NL 
		$serverAttribute::integer;

$serverAttribute[$serverAttribute::count++] 
	=	"Elevators Enabled" NL 
		"Server::EnabledElevator" NL 
		$serverAttribute::boolean;
		
$serverAttribute[$serverAttribute::count++] 
	=	"Decals Enabled" NL 
		"Server::SCommands" NL 
		$serverAttribute::boolean;
		
$serverAttribute[$serverAttribute::count++] 
	=	"Auto Secure Enabled" NL 
		"Server::AutoSecure" NL 
		$serverAttribute::boolean;
		
$serverAttribute[$serverAttribute::count++] 
	=	"Save Brick Owners Enabled" NL 
		"Server::SaveBrickOwnersOnExit" NL 
		$serverAttribute::boolean;
		
$serverAttribute[$serverAttribute::count++] 
	=	"Map Items Enabled" NL 
		"Server::MaxAllowedBricks" NL 
		$serverAttribute::integer;		