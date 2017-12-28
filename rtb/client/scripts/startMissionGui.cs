// --------------------------------
// UI Functions


// One of the old RTB/v2 devs wrote this comment.
// I'm keeping it around because I think it's funny.
function startMissionGui::onSleep(%this)
{
	//save the server prefs from the screen
	//will this work? we might have started the server by the time this gets called.
	//UPDATE: nope, it doesnt
	

}

function startMissionGui::onWake(%this) {
	// --------------------------------
	// Populate attributes list using serverAttributes.cs
	
	// Clear everything first...
	%nameCtrl = nameToID(startMissionGui_ctrlSettingList);
	%valCtrl  = nameToID(startMissionGui_ctrlValueList);
	%haxBox   = nameToID(startMissionGui_scrlAttributesHax);
	
	%haxBox.clear();
	
	%nameCtrl.clear();
	%nameCtrl.extent = getWord(%nameCtrl.extent, 0) SPC 2;
	
	%valCtrl.clear();
	%valCtrl.extent = getWord(%nameCtrl.extent, 0) SPC 2;
	
	// Check if there are any attributes.
	if($serverAttribute::count < 1)
		return;
	
	// Add rows accordingly.
	for(%i = 0; %i < $serverAttribute::count; %i++)
		%this.addAttributeRow(%i);
	
	// Since the scrollbox doesn't automatically resize, we have to use a 
	// hacky method to force it into doing so.
	
	for(%i = 0; getWord(%haxBox.extent, 1) < getWord(%nameCtrl.extent, 1) + 10; %i++)
		%haxBox.addRow(%i, "bunny");
	
	// --------------------------------
	// Load all maps
	
	%mapList = nameToId(startMissionGui_lstMaps);
	%i       = 0;
	
	// Clear them first.
	
	%mapList.clear();
	
	for(%file = findFirstFile($Server::MissionFileSpec); 
		%file !$= ""; 
		%file = findNextFile($Server::MissionFileSpec)) {
		
		%this.mapInfoObjects[%i] = %this.getMissionCodeBlock(%file);
		%this.mapInfoObjects[%i].file = %file;
		
		%mapList.addRow(%i, %this.getMissionName(%this.mapInfoObjects[%i])); 
		%i++;
	}
	
	%mapList.sort(0);
	
	// --------------------------------
	// Load server settings
	
	nameToID(startMissionGui_txtServerName).setText($Pref::Server::BaseServerName);
	nameToID(startMissionGui_txtPort).setText($Pref::Server::Port);
	nameToID(startMissionGui_TxtServerInfo).setText($Pref::Server::Info);
	nameToID(startMissionGui_TxtServerMOTD).setText($Pref::Server::MOTD);
	nameToID(startMissionGui_TxtServerPW).setText($Pref::Server::Password);
	nameToID(startMissionGui_txtAdminPW).setText($Pref::Server::AdminPassword);
	nameToID(startMissionGui_txtTAdminPW).setText($Pref::Server::TempAdminPassword);
	nameToID(startMissionGui_txtBuilderPW).setText($Pref::Server::BuilderPassword);
	nameToID(startMissionGui_chkHidePWs).setValue($Pref::Client::HideStartMissionGuiPassword);
	nameToID(startMissionGui_numMaxPlayers).setValue($Pref::Server::MaxPlayers);
	
	// --------------------------------
	// Load Autosave settings
	
	nameToID(startMissionGui_chkAutosaveEnabled).setValue($Pref::Server::Autosave::Enabled);
	nameToID(startMissionGui_numSaveInterval).setValue($Pref::Server::Autosave::Interval);
	nameToID(startMissionGui_txtAutosaveName).setText($Pref::Server::Autosave::Name);
	nameToID(startMissionGui_chkIncremental).setValue($Pref::Server::Autosave::Incremental);
	
	switch$($Pref::Server::Autosave::Type) {
		case "Persistence": nameToID(startMissionGui_rdioPersistence).setValue(true);
		case "BunnySave":   nameToID(startMissionGui_rdioBSave).setValue(true);
		case "Both":        nameToID(startMissionGui_rdioBoth).setValue(true);
	}
	
	// --------------------------------
	// Set default visual settings
	
	// Select the default tab
	nameToID(startMissionGui_btnServer).setValue(false);
	nameToID(startMissionGui_btnAttributes).setValue(false);
	nameToID(startMissionGui_btnAutosave).setValue(false);
	
	
	%this.selectedTab = -1;
	%this.tabChanged(0);
	nameToID(startMissionGui_btnMap).setValue(true);
	
	// Select the default map
	nameToID(startMissionGui_lstMaps).setSelectedRow(0);
	
	
	// Update password hiding
	%this.onHidePWtoggle();
	
	
	// Update overviews
	
	%this.updateOverview0();
	%this.updateOverview1();
}

function startMissionGui::startMission(%this, %serverType) {
	%this.updatePrefs();
	
	$Pref::Server::Name = "[RTB]" SPC $Pref::Server::BaseServerName;
	$Client::Password = $Pref::Server::Password;
	
	if(%serverType $= "")
		%serverType = "SinglePlayer";
	
	createServer(%serverType, %this.selectedMap.file);
	
	echo("Creating server.");
	
	%conn = new GameConnection(ServerConnection);
	RootGroup.add(ServerConnection);
	%conn.setConnectArgs($pref::Player::Name);
	%conn.setJoinPassword($Client::Password);
	echo(" - Connection:" SPC %conn);
	%conn.connectLocal();
}

function startMissionGui::updatePrefs(%this) {
	// Save values that are static.
	
	$Pref::Server::BaseServerName              = nameToID(startMissionGui_txtServerName).getValue();
	$Pref::Server::Port                        = nameToID(startMissionGui_txtPort).getValue();
	$Pref::Server::Info                        = nameToID(startMissionGui_TxtServerInfo).getValue(); 
	$Pref::Server::MOTD                        = nameToID(startMissionGui_TxtServerMOTD).getValue();
	$Pref::Server::Password                    = nameToID(startMissionGui_TxtServerPW).getValue();
	$Pref::Server::AdminPassword               = nameToID(startMissionGui_txtAdminPW).getValue();
	$Pref::Server::TempAdminPassword           = nameToID(startMissionGui_txtTAdminPW).getValue();
	$Pref::Server::BuilderPassword             = nameToID(startMissionGui_txtBuilderPW).getValue();
	$Pref::Client::HideStartMissionGuiPassword = nameToID(startMissionGui_chkHidePWs).getValue();
	$Pref::Server::MaxPlayers                  = nameToID(startMissionGui_numMaxPlayers).getValue();
	$Pref::Server::Autosave::Enabled           = nameToID(startMissionGui_chkAutosaveEnabled).getValue();
	$Pref::Server::Autosave::Interval          = nameToID(startMissionGui_numSaveInterval).getValue();
	$Pref::Server::Autosave::Name              = nameToID(startMissionGui_txtAutosaveName).getValue();
	$Pref::Server::Autosave::Incremental       = nameToID(startMissionGui_chkIncremental).getValue();
	
	%saveType = "Both";
	
	if(nameToID(startMissionGui_rdioPersistence).getValue())
		%saveType = "Persistence";
	if(nameToID(startMissionGui_rdioBSave).getValue())
		%saveType = "BunnySave";
	
	$Pref::Server::Autosave::Type = %saveType;
	
	// Save values that are less static.
	
	%valList = nameToID(startMissionGui_ctrlValueList);
	
	for(%i = 0; %i < $serverAttribute::count; %i++) {
		%ctrl = %valList.getObject(%i);
				
		%this.setPrefFromString(%ctrl.prefName, %ctrl.getValue());
	}
}

function startMissionGui::updateOverview0(%this) {
	%pw = nameToID(startMissionGui_TxtServerPW).getValue();
	%label = nameToID(startMissionGui_lblOVServerPW);	
	
	// Password
	
	if(nameToID(startMissionGui_chkHidePWs).getValue()) {
		%str = "";
		for(%i = 0; %i < strLen(%pw); %i++)
			%str = %str @ "*";
		%label.setText("Password:" SPC %str);
	} else {
		%label.setText("Password:" SPC %pw);
	}
	
	nameToID(startMissionGui_lblOVServerMap).setText("Map:" SPC %this.getMissionName(%this.selectedMap));
	nameToID(startMissionGui_lblOVServerName).setText("Name: [RTB]" SPC nameToID(startMissionGui_txtServerName).getValue());
}

function startMissionGui::updateOverview1(%this) {
	nameToID(startMissionGui_lblOVServerPort).setText("Port:" SPC nameToID(startMissionGui_txtPort).getValue());
}

function startMissionGui::onHidePWtoggle(%this) {
	%active = nameToID(startMissionGui_chkHidePWs).getValue();
	
	nameToID(startMissionGui_TxtServerPW).password = %active;
	nameToID(startMissionGui_txtAdminPW).password = %active;
	nameToID(startMissionGui_txtTAdminPW).password = %active;
	nameToID(startMissionGui_txtBuilderPW).password = %active;
}

// --------------------------------
// Tab Changing Functions

function startMissionGui::tabChanged(%this, %index) {
	
	if(%index == %this.selectedTab)
		return;
	
	error("test");
	
	%j = 0;
	%tab[%j]   = nameToId("startMissionGui_ctrlMapTab");
	%tab[%j++] = nameToId("startMissionGui_ctrlServerTab");
	%tab[%j++] = nameToId("startMissionGui_ctrlAttributesTab");
	%tab[%j++] = nameToId("startMissionGui_ctrlAutosaveTab");
	
	%curTab = %tab[%index];
	
	// Check if the tab exists.
	if(!isObject(%curTab)) 
		return;
		
	// bunnyReveal selected control
	%curTab.bunnyReveal();
	%curTab.setVisible(true);
	
	// bunnyHide everything else
	for(%i = 0; %i <= %j; %i++)
		if(%i != %index)
			%tab[%i].bunnyHide();
	
	%this.selectedTab = %index;
}

// --------------------------------
// Map Tab Functions

function startMissionGui::mapIndexChanged(%this) {
	%ctrl = nameToID(startMissionGui_lstMaps);
	
	%mapInfoObject = %this.mapInfoObjects[%ctrl.getSelectedId()];
	
	// Change preview bitmap
	
	if(!isObject(%mapInfoObject))
		return;
	
	
	// Update the description.
	nameToID(startMissionGui_imgMapPreview).setBitmap("rtb/data/missions/previews/" @ fileBase(%mapInfoObject.file) @ ".png");
	
	nameToID(startMissionGui_lblMapDescription).setText(%this.getMissionDescription(%mapInfoObject));
	
	
	// Update first column of map overview
	
	%this.selectedMap = %mapInfoObject;
	%this.updateOverview0();
	
	// Update save list.
	
	%ctrl = nameToID(startMissionGui_lstSaves);
	
	%ctrl.clear();
	%ctrl.addRow(0, " (none)");
	
	%savePattern = %mapInfoObject.file @ "_*.*";
	
	%i = 1;
	for(%file = findFirstFile(%savePattern); 
		%file !$= ""; 
		%file = findNextFile(%savePattern)) {
			%fileName = fileName(%file);
			error(%fileName);
			
			%ctrl.addRow(%i, getSubStr(
								strChr(%fileName, "_"),
								1,
								strLen(%fileName)));
			
			%this.mapSavePath[%i - 1] = %file;
			%i++;
		}
	
	%ctrl.setSelectedRow(0);
}

function startMissionGui::saveIndexChanged(%this) {
	%index = nameToID(startMissionGui_lstSaves).getSelectedId();
	%descriptionCtrl = nameToID(startMissionGui_lblSaveDescription);
	// remember: the first object in the list is the " (none)" option.
	%saveFile = %this.mapSavePath[%index - 1];
	
	// clear descriptionCtrl in anticipation of filling it; if selection is invalid,
	// nothing changes.
	
	%descriptionCtrl.setText("");
	
	if(%index == 0)
		return;
	else if(!isFile(%saveFile))
		return;
		
	%this.selectedSave = %saveFile;
	
	// undescriptive name for persistence files, because there's no fucking way I'm touching that goddamn mess.
	%descriptionText = "Persistence file.";
	
	error("X" SPC %saveFile);
	if(fileExt(%saveFile) $= ".bsave") {
		// Not implemented.
		%descriptionText = ".bsave code not yet implemented.";
	}
	
	%descriptionCtrl.setText(%descriptionText);
}

function startMissionGui::getMissionName(%this, %missionInfoObject) {
	if( %MissionInfoObject.name !$= "" )
		return %MissionInfoObject.name;
	else
		return fileBase(%missionFile); 
}

function startMissionGui::getMissionDescription(%this, %missionInfoObject) {
	
	if(%missionInfoObject.desc0 $= "")
		return "No description.";
	
	%totalDesc = "";
	for(%i = 0; %i < %missionInfoObject.descLines; %i++)
		%totalDesc = %totalDesc @ %missionInfoObject.desc[%i] @ "\n";
	
	return %totalDesc;
}

// --------------------------------
// Attribute Tab Functions

startMissionGUI.minAttributeHeight  =  18;
startMissionGUI.minAttributePadding =   4;

function startMissionGui::addAttributeRow(%this, %attributeIndex) {
	
	%att = $serverAttribute[%attributeIndex];
	
	%attName = getField(%att, 0); // Is it empty?
	if(strLen(%attName) == 0)
		return;
		
	%attPref = getField(%att, 1); // Is it empty?
	if(strLen(%attPref) == 0)
		return;
	
	%attValType = getField(%att, 2); // Is it empty?
	if(strLen(%attValType) == 0)
		return;	
		
	%nameCtrl = %this.getAttributeNameControl(%attName);
	%valCtrl  = %this.getAttributeValueControl(%this.getPrefFromString(%attPref), %attValType);
	
	%cellSize = %this.minAttributeHeight + %this.minAttributePadding;
	%ctrlYPos = %attributeIndex * %cellSize;
		
	
	%ctrlSettingList = nameToID(startMissionGui_ctrlSettingList);
	%ctrlValueList   = nameToID(startMissionGui_ctrlValueList);
	// Parent and position the Name column control
	%nameCtrl.position = "4" SPC %ctrlYPos;
	%ctrlSettingList.add(%nameCtrl);
	
	// Parent and position the Value column control.
	%valCtrl.position = "0" SPC %ctrlYPos;
	%ctrlValueList.add(%valCtrl);
	
	%valCtrl.prefName = %attPref;
	
	// Resize the height of both gui containers.
	%colHeight = %cellSize * (%attributeIndex + 1);
	
	%ctrlSettingList.extent = getWord(%ctrlSettingList.extent, 0) SPC %colHeight;
	%ctrlValueList.extent   = getWord(%ctrlValueList.extent, 0) SPC %colHeight;
}

function startMissionGui::getAttributeValueControl(%this, %prefValue, %attValType) {
	
	
	switch(%attValType) {
		case $serverAttribute::boolean: 
			%control = new GuiCheckBoxCtrl() {
				profile		= "GuiCheckBoxProfile";
				horizSizing	= "right";
				vertSizing	= "bottom";
				groupNum	= "-1";
				buttonType	= "ToggleButton";
				text		= "";
				
				extent		= "18" SPC %this.minAttributeHeight;
				minExtent	= "18" SPC %this.minAttributeHeight;
			};
			
			%control.setValue(%prefValue);
			
			return %control;

		case $serverAttribute::integer:
             %control = new GuiTextEditCtrl() {
					profile			= "GuiTextEditProfile";
					horizSizing		= "right";
					vertSizing		= "bottom";
					maxLength		= "255";
					historySize		= "0";
					password		= "0";
					tabComplete		= "0";
					sinkAllKeyEvents= "0";
					
					minExtent		= "36" SPC %this.minAttributeHeight;
					text			= %prefValue;
               };
			   
			   return %control;
	}
}

startMissionGui.ninetyDots = "..........................................................................................";

function startMissionGui::getAttributeNameControl(%this, %attName) {
   %control = new GuiTextCtrl() {
		profile		= "GuiTextProfile";
		horizSizing	= "right";
		vertSizing	= "bottom";
		helpTag		= "0";
		maxLength	= "255";
		
		minExtent	= "4" SPC %this.minAttributeHeight;
	};
	
	%control.setText(%attName SPC %this.ninetyDots);
	
	return %control;
}

function startMissionGui::getPrefFromString(%this, %prefStr) {
	return eval("return $pref::" @ %prefStr @ ";");
}

function startMissionGui::setPrefFromString(%this, %prefStr, %value) {
	eval("$pref::" @ %prefStr @ " = \"" @ %value @ "\";");
}

//----------------------------------------
// Modified getMissionName() code

function startMissionGui::getMissionCodeBlock(%this, %missionFile) 
{
	%file = new FileObject();
	
	%MissionInfoObject = "";
	
	if(%file.openForRead(%missionFile)) {
		%inInfoBlock = false;
			
		while(!%file.isEOF()) {
			%line = %file.readLine();
			%line = trim(%line);
			
			if(%line $= "new ScriptObject(MissionInfo) {")
				%inInfoBlock = true;
			else if(%inInfoBlock && %line $= "};") {
				%inInfoBlock = false;
				%MissionInfoObject = %MissionInfoObject @ %line; 
				break;
			}
			
			if(%inInfoBlock)
				%MissionInfoObject = %MissionInfoObject @ %line @ " "; 	
		}
		
		%file.close();
	}
	%MissionInfoObject = "%MissionInfoObject = " @ %MissionInfoObject;
	eval(%MissionInfoObject);
	
	%file.delete();
	
	return %MissionInfoObject;
}