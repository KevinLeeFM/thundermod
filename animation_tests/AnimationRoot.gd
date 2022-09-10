extends Spatial


onready var animation_player: AnimationPlayer = $AnimationPlayer


# Called when the node enters the scene tree for the first time.
func _ready():
    pass # Replace with function body.


func _input(event):
    if event.is_action_pressed("move_right"):
        animation_player.play("AnimationA1")
    else:
        animation_player.play("AnimationB1")
