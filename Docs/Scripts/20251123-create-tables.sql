--CREATE EXTENSION IF NOT EXISTS vector;

CREATE TABLE qa_interactions (
	id serial4 NOT NULL,
	visitor_id text NULL,
	question text NOT NULL,
	answer text NOT NULL,
	interaction_time timestamp NULL,
	user_ip text NULL,
	CONSTRAINT qa_interactions_pkey PRIMARY KEY (id)
);

-- public.resume_chunks definition

-- Drop table

-- DROP TABLE resume_chunks;

CREATE TABLE resume_chunks (
	id serial4 NOT NULL,
	chunk_text text NULL,
	embedding public.vector NULL,
	CONSTRAINT resume_chunks_pkey PRIMARY KEY (id)
);