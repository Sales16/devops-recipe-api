from fastapi import FastAPI, Depends, HTTPException
from sqlalchemy.orm import Session
from database import engine, SessionLocal, Base
from models import Ingrediente as IngredienteModel
from schemas import IngredienteCreate, Ingrediente
from sqlalchemy.exc import SQLAlchemyError
from typing import List

Base.metadata.create_all(bind=engine)

app = FastAPI()

def get_db():
    try:
        db = SessionLocal()
        yield db
    finally:
        db.close()

@app.post("/ingredientes/")
def criar_ingrediente(ingrediente: IngredienteCreate, db: Session = Depends(get_db)):
    try:
        existente = db.query(Ingrediente).filter(Ingrediente.nome == ingrediente.nome).first()
        if existente:
            raise HTTPException(status_code=400, detail="Ingrediente já existe.")

        novo = Ingrediente(nome=ingrediente.nome, unidade=ingrediente.unidade)
        db.add(novo)
        db.commit()
        db.refresh(novo)
        return {"mensagem": "Ingrediente criado com sucesso!", "id": novo.id, "nome": novo.nome, "unidade": novo.unidade}

    except SQLAlchemyError as e:
        raise HTTPException(status_code=500, detail=f"Erro no banco: {e}")
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Erro inesperado: {e}")


@app.get("/ingredientes/", response_model=List[Ingrediente])
def listar_ingredientes(db: Session = Depends(get_db)):
    try:
        ingredientes = db.query(IngredienteModel).all()
        return ingredientes
    except SQLAlchemyError as e:
        raise HTTPException(status_code=500, detail=f"Erro no banco: {e}")
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Erro inesperado: {e}")