from functools import wraps
import inspect
import math

import sqlite3
from sqlite3 import Cursor
import flask

app = flask.Flask(__name__)

# Looks at the function's type hints to fill in the corresponding arguments
# from the request.form object, converting the strings to the types specified.
def fill_params_from_form(fn):
	@wraps(fn)
	def inner():
		args = {}
		conn, cur = None, None
		for param, info in inspect.signature(fn).parameters.items():
			ty = str if info.annotation == inspect.Parameter.empty else info.annotation
			if ty == sqlite3.Cursor: # lol
				if conn is None or cur is None:
					conn = sqlite3.connect("scores.db")
					cur = conn.cursor()
				args[param] = cur
			elif ty == list:
				# special case: return a list of strings
				args[param] = flask.request.form.getlist(param)
			else:
				args[param] = flask.request.form.get(param, type=ty)
		r = fn(**args)
		if cur is not None:
			cur.close()
		if conn is not None:
			conn.commit()
			conn.close()
		return r
	return inner

@app.post("/score/submit")
@fill_params_from_form
def highscore(cur: Cursor, level: str, time: float, name: str, accuracy: bool):
	try:
		if not math.isfinite(time)\
		or time < 0.5:
			raise Exception(":  )")
		
		name = ascii(name.strip())[1:-1]
		
		cur.execute("""
			SELECT time
			FROM scores
			WHERE level = ?
			AND name = ?
			AND accuracy = ?
			LIMIT 1;
		""", (level, name, accuracy))
		prevTime = cur.fetchone()
		
		code = 205
		
		if prevTime is None \
		or time < prevTime[0]:
			cur.execute("""
				REPLACE INTO scores
				(level, time, name, accuracy)
				VALUES ( ?, ?, ?, ? );
			""", (level, time, name, accuracy))
			code = 200
		
		cur.execute("""
			SELECT name, time, accuracy
			FROM scores
			WHERE level = ?
			ORDER BY time ASC
			LIMIT 5;
		""", (level,))
		formatted = "\n".join([
			f"{'→' if name == s_name else ' '} {i}: {s_name} :: {round(time, 2):.2f}s{'*' if accuracy else ''}"
			for (i, (s_name, time, accuracy))
			in enumerate(cur.fetchall(), start=1)
		])
		return formatted, code
	except Exception as e:
		print(repr(e.__traceback__), repr(e))
		return "", 500

if __name__ == '__main__':
	conn = sqlite3.connect("scores.db")
	conn.executescript("""
	CREATE TABLE IF NOT EXISTS scores (
		level VARCHAR(64) NOT NULL,
		name VARCHAR(8) NOT NULL,
		accuracy BOOLEAN NOT NULL,
		
		time REAL NOT NULL,
		
		PRIMARY KEY (level, name, accuracy)
	);""")
	conn.close()
	
	app.run(port=8000)
