/// Bunny RTB - serverAttributes.cs
/// made by emiko/slate5521
//
// This code is where UI elements are stored, thus making them unalterable. It
// is useful for rotating controls/tabs, or any control you need to hide 
// without destroying it.

// "Hide" the element by placing it inside the bunnyControlHolder GUI
function GuiControl::bunnyHide(%this) {
	// If it does have an original parent, then it (very likely) is hidden.
	if(isObject(%this.originalParent))
		return;
		
	%this.originalParent = %this.getGroup();
	nameToID(bunnyControlHolder).add(%this);
}

// "Reveal" the element by placing it back in its previous parent.
function GuiControl::bunnyReveal(%this) {
	// If it doesn't have an original parent, then it (very likely) isn't hidden.
	if(!isObject(%this.originalParent))
		return;
		
	%this.originalParent.add(%this);
	%this.originalParent = -1;
}