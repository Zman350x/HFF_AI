import json

class GameInputs:
	def __init__(self):
		self.look_x = 0.0
		self.look_y = 0.0
		self.jump = False
		self.arm_left = False
		self.arm_right = False
		self.forward = False
		self.backward = False
		self.left = False
		self.right = False

	def format_json(self) -> str:
		return json.dumps(self, default=lambda o: o.__dict__)