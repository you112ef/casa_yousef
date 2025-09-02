# PowerShell script to create essential medical lab tables
Write-Host "CREATING ESSENTIAL MEDICAL LAB TABLES" -ForegroundColor Green
Write-Host "===================================" -ForegroundColor Green
Write-Host ""

# Try to load the SQLite assembly
try {
    Add-Type -Path "System.Data.SQLite.dll"
    Write-Host "SQLite assembly loaded successfully." -ForegroundColor Green
} catch {
    Write-Host "Failed to load SQLite assembly:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    exit 1
}

# Function to create a table
function Create-Table {
    param(
        [string]$tableName,
        [string]$sql
    )
    
    try {
        $connectionString = "Data Source=database.db;Version=3;"
        $connection = New-Object System.Data.SQLite.SQLiteConnection($connectionString)
        $connection.Open()
        
        # Check if table already exists
        $command = New-Object System.Data.SQLite.SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' AND name='$tableName'", $connection)
        $reader = $command.ExecuteReader()
        $tableExists = $reader.HasRows
        $reader.Close()
        
        if (-not $tableExists) {
            Write-Host "Creating table: $tableName" -ForegroundColor Yellow
            $command = New-Object System.Data.SQLite.SQLiteCommand($sql, $connection)
            $command.ExecuteNonQuery()
            Write-Host "  Table $tableName created successfully!" -ForegroundColor Green
        } else {
            Write-Host "Table $tableName already exists." -ForegroundColor Green
        }
        
        $connection.Close()
    } catch {
        Write-Host ("  Error creating table " + $tableName + ":") -ForegroundColor Red
        Write-Host ("  " + $_.Exception.Message) -ForegroundColor Red
        if ($connection) { $connection.Close() }
    }
}

# Create patients table
$patientsSQL = @"
CREATE TABLE patients (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    first_name TEXT NOT NULL,
    last_name TEXT NOT NULL,
    date_of_birth DATE,
    gender TEXT,
    phone TEXT,
    address TEXT,
    registration_date DATETIME DEFAULT CURRENT_TIMESTAMP
);
CREATE INDEX idx_patients_last_name ON patients(last_name);
CREATE INDEX idx_patients_registration_date ON patients(registration_date);
"@
Create-Table "patients" $patientsSQL

# Create doctors table
$doctorsSQL = @"
CREATE TABLE doctors (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    first_name TEXT NOT NULL,
    last_name TEXT NOT NULL,
    specialty TEXT,
    phone TEXT,
    email TEXT,
    created_date DATETIME DEFAULT CURRENT_TIMESTAMP
);
CREATE INDEX idx_doctors_last_name ON doctors(last_name);
"@
Create-Table "doctors" $doctorsSQL

# Create admin table
$adminSQL = @"
CREATE TABLE admin (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    username TEXT UNIQUE NOT NULL,
    password TEXT NOT NULL,
    role TEXT NOT NULL,
    created_date DATETIME DEFAULT CURRENT_TIMESTAMP
);
CREATE INDEX idx_admin_username ON admin(username);
"@
Create-Table "admin" $adminSQL

# Create cbc_results table
$cbcSQL = @"
CREATE TABLE cbc_results (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    patient_id INTEGER NOT NULL,
    doctor_id INTEGER,
    test_date DATETIME DEFAULT CURRENT_TIMESTAMP,
    hemoglobin REAL,
    hematocrit REAL,
    wbc_count REAL,
    rbc_count REAL,
    platelet_count REAL,
    mcv REAL,
    mch REAL,
    mchc REAL,
    rdw REAL,
    total_lymphocytes REAL,
    total_monocytes REAL,
    total_basophils REAL,
    total_eosinophils REAL,
    total_neutrophils REAL,
    total_wbc REAL,
    total_rbc REAL,
    total_platelets REAL,
    plt_histogram_width REAL,
    plt_mean_size REAL,
    plt_large_cell_ratio REAL,
    plt_small_cell_ratio REAL,
    plt_volume REAL,
    plt_volume_distribution_width REAL,
    plt_volume_mean REAL,
    plt_volume_standard_deviation REAL,
    plt_volume_variance REAL,
    plt_width REAL,
    plt_width_distribution_width REAL,
    plt_width_mean REAL,
    plt_width_standard_deviation REAL,
    plt_width_variance REAL,
    plt_width_width REAL,
    FOREIGN KEY (patient_id) REFERENCES patients(id),
    FOREIGN KEY (doctor_id) REFERENCES doctors(id)
);
CREATE INDEX idx_cbc_results_patient_id ON cbc_results(patient_id);
CREATE INDEX idx_cbc_results_test_date ON cbc_results(test_date);
"@
Create-Table "cbc_results" $cbcSQL

# Add default admin user
try {
    $connectionString = "Data Source=database.db;Version=3;"
    $connection = New-Object System.Data.SQLite.SQLiteConnection($connectionString)
    $connection.Open()
    
    # Check if admin table exists and is empty
    $command = New-Object System.Data.SQLite.SQLiteCommand("SELECT COUNT(*) FROM admin", $connection)
    $count = $command.ExecuteScalar()
    
    if ($count -eq 0) {
        Write-Host "Adding default admin user..." -ForegroundColor Yellow
        $insertSQL = "INSERT INTO admin (username, password, role) VALUES ('admin', 'admin123', 'administrator')"
        $command = New-Object System.Data.SQLite.SQLiteCommand($insertSQL, $connection)
        $command.ExecuteNonQuery()
        Write-Host "  Default admin user added." -ForegroundColor Green
    } else {
        Write-Host "Admin table already has data." -ForegroundColor Green
    }
    
    $connection.Close()
} catch {
    Write-Host "Error adding default admin user:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    if ($connection) { $connection.Close() }
}

# Add sample doctor
try {
    $connectionString = "Data Source=database.db;Version=3;"
    $connection = New-Object System.Data.SQLite.SQLiteConnection($connectionString)
    $connection.Open()
    
    # Check if doctors table exists and is empty
    $command = New-Object System.Data.SQLite.SQLiteCommand("SELECT COUNT(*) FROM doctors", $connection)
    $count = $command.ExecuteScalar()
    
    if ($count -eq 0) {
        Write-Host "Adding sample doctor..." -ForegroundColor Yellow
        $insertSQL = "INSERT INTO doctors (first_name, last_name, specialty) VALUES ('John', 'Smith', 'Pathologist')"
        $command = New-Object System.Data.SQLite.SQLiteCommand($insertSQL, $connection)
        $command.ExecuteNonQuery()
        Write-Host "  Sample doctor added." -ForegroundColor Green
    } else {
        Write-Host "Doctors table already has data." -ForegroundColor Green
    }
    
    $connection.Close()
} catch {
    Write-Host "Error adding sample doctor:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    if ($connection) { $connection.Close() }
}

Write-Host ""
Write-Host "ESSENTIAL TABLE CREATION COMPLETE" -ForegroundColor Green
Write-Host "================================" -ForegroundColor Green