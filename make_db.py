import sqlite3, os
base = os.path.dirname(__file__)
payload_dir = os.path.join(base, 'installer-build', 'payload')
os.makedirs(payload_dir, exist_ok=True)
db_path = os.path.join(payload_dir, 'database.db')
conn = sqlite3.connect(db_path)
c = conn.cursor()
# Core tables
c.executescript('''
CREATE TABLE IF NOT EXISTS admin (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  username TEXT NOT NULL UNIQUE,
  password TEXT NOT NULL,
  role TEXT DEFAULT 'admin',
  created_date DATETIME DEFAULT CURRENT_TIMESTAMP
);
CREATE TABLE IF NOT EXISTS patients (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  name TEXT NOT NULL,
  birth_date DATETIME,
  gender TEXT,
  phone_number TEXT,
  address TEXT,
  created_date DATETIME DEFAULT CURRENT_TIMESTAMP
);
CREATE TABLE IF NOT EXISTS cbc (
  test_result_id INTEGER PRIMARY KEY AUTOINCREMENT,
  patient_id INTEGER NOT NULL,
  test_date DATETIME DEFAULT CURRENT_TIMESTAMP,
  wbc REAL, rbc REAL, hemoglobin REAL, hematocrit REAL,
  FOREIGN KEY(patient_id) REFERENCES patients(id)
);
CREATE TABLE IF NOT EXISTS urine (
  test_result_id INTEGER PRIMARY KEY AUTOINCREMENT,
  patient_id INTEGER NOT NULL,
  test_date DATETIME DEFAULT CURRENT_TIMESTAMP,
  color TEXT, ph REAL,
  FOREIGN KEY(patient_id) REFERENCES patients(id)
);
CREATE TABLE IF NOT EXISTS stool (
  test_result_id INTEGER PRIMARY KEY AUTOINCREMENT,
  patient_id INTEGER NOT NULL,
  test_date DATETIME DEFAULT CURRENT_TIMESTAMP,
  color TEXT, ph REAL,
  FOREIGN KEY(patient_id) REFERENCES patients(id)
);
CREATE TABLE IF NOT EXISTS kidney_function (
  test_result_id INTEGER PRIMARY KEY AUTOINCREMENT,
  patient_id INTEGER NOT NULL,
  test_date DATETIME DEFAULT CURRENT_TIMESTAMP,
  creatinine REAL, bun REAL,
  FOREIGN KEY(patient_id) REFERENCES patients(id)
);
CREATE TABLE IF NOT EXISTS liver_function (
  test_result_id INTEGER PRIMARY KEY AUTOINCREMENT,
  patient_id INTEGER NOT NULL,
  test_date DATETIME DEFAULT CURRENT_TIMESTAMP,
  alt REAL, ast REAL,
  FOREIGN KEY(patient_id) REFERENCES patients(id)
);
CREATE TABLE IF NOT EXISTS semen_analysis (
  test_result_id INTEGER PRIMARY KEY AUTOINCREMENT,
  patient_id INTEGER NOT NULL,
  test_date DATETIME DEFAULT CURRENT_TIMESTAMP,
  volume_ml REAL, ph REAL,
  ai_analysis_performed INTEGER DEFAULT 0,
  ai_confidence_score REAL,
  rapid_progressive_percent REAL,
  slow_progressive_percent REAL,
  non_progressive_percent REAL,
  immotile_percent REAL,
  analyzed_image_path TEXT,
  heatmap_image_path TEXT,
  FOREIGN KEY(patient_id) REFERENCES patients(id)
);
''')
# Seed admin
c.execute("SELECT COUNT(*) FROM admin")
if c.fetchone()[0] == 0:
    c.execute("INSERT INTO admin (username,password,role) VALUES (?,?,?)", ("admin","admin","admin"))
# Seed patients
c.execute("SELECT COUNT(*) FROM patients")
if c.fetchone()[0] == 0:
    c.execute("INSERT INTO patients (name, birth_date, gender, phone_number, address) VALUES (?,?,?,?,?)",
              ("Demo Patient","1990-01-01","ذكر","01000000000",""))
conn.commit()
conn.close()
print("Created:", db_path)
