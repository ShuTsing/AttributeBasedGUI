@tool
extends EditorPlugin


func _enter_tree():
	
	#var string_field_script = preload("res://addons/AttributeBasedGUI/Scripts/AttributeControls/PropertyFields/StringField.cs")
	#var string_field_icon = preload("res://addons/AttributeBasedGUI/Resources/Arts/Textures/Icons/input-field.png")
	#add_custom_type("StringField", "Control", string_field_script, string_field_icon)
	
	var window_container_script = preload("res://addons/AttributeBasedGUI/Scripts/Windows/WindowContainer.cs")
	var window_container_icon = preload("res://addons/AttributeBasedGUI/Resources/Arts/Textures/Icons/window-line.png")
	add_custom_type("WindowContainer", "MarginContainer", window_container_script, window_container_icon)
	
	var class_window_container_script = preload("res://addons/AttributeBasedGUI/Scripts/Windows/ClassWindowContainer.cs")
	var class_window_container_icon = preload("res://addons/AttributeBasedGUI/Resources/Arts/Textures/Icons/window-line.png")
	add_custom_type("ClassWindowContainer", "MarginContainer", class_window_container_script, class_window_container_icon)
	
	pass


func _exit_tree():
	# Clean-up of the plugin goes here.
	
	remove_custom_type("ClassWindowContainer")
	remove_custom_type("WindowContainer")
	pass
