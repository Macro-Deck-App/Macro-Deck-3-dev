CREATE TABLE folder
(
    f_id                INTEGER NOT NULL
        CONSTRAINT pk_folder PRIMARY KEY AUTOINCREMENT,
    f_display_name      TEXT    NOT NULL,
    f_index             INT     NOT NULL,
    f_parent_folder_ref INT REFERENCES folder (f_id) ON DELETE CASCADE,
    f_tree              TEXT    NOT NULL,
    f_row_count         INT     NOT NULL,
    f_column_count      INT     NOT NULL,
    f_background_color  TEXT,
    f_create_timestamp  TEXT    NOT NULL,
    f_update_timestamp  TEXT
);
CREATE INDEX idx_folder_parent_folder_ref ON folder (f_parent_folder_ref);
CREATE INDEX idx_folder_tree ON folder (f_tree);

CREATE TABLE widget
(
    w_id                         INTEGER NOT NULL
        CONSTRAINT pk_widget PRIMARY KEY AUTOINCREMENT,
    w_folder_ref                 INTEGER NOT NULL REFERENCES folder (f_id) ON DELETE CASCADE,
    w_type                       INT     NOT NULL,
    w_row                        INT     NOT NULL,
    w_row_span                   INT     NOT NULL,
    w_increase_row_span_possible BOOLEAN NOT NULL,
    w_decrease_row_span_possible BOOLEAN NOT NULL,
    w_column                     INT     NOT NULL,
    w_col_span                   INT     NOT NULL,
    w_increase_col_span_possible BOOLEAN NOT NULL,
    w_decrease_col_span_possible BOOLEAN NOT NULL,
    w_data                       TEXT,
    w_create_timestamp           TEXT    NOT NULL,
    w_update_timestamp           TEXT
);
CREATE INDEX idx_widget_folder_ref ON widget (w_folder_ref);
CREATE INDEX idx_widget_row_column ON widget (w_row, w_column);

CREATE TABLE integration_configuration
(
    ic_id                     INTEGER NOT NULL
        CONSTRAINT pk_integration_configuration PRIMARY KEY AUTOINCREMENT,
    ic_integration_identifier INT     NOT NULL,
    ic_key                    INT     NOT NULL,
    ic_value                  BLOB    NOT NULL,
    ic_public                 BOOLEAN DEFAULT false,
    ic_create_timestamp       TEXT    NOT NULL,
    ic_update_timestamp       TEXT
);
CREATE INDEX idx_integration_configuration_integration_identifier ON integration_configuration (ic_integration_identifier);
CREATE INDEX idx_integration_configuration_integration_identifier_key ON integration_configuration (ic_integration_identifier, ic_key);