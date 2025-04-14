from fastapi import FastAPI, Depends, HTTPException
from sqlalchemy.orm import Session
from database import engine, SessionLocal, Base
from models import Ingrediente as IngredienteModel
from schemas import IngredienteCreate, Ingrediente, IngredienteUpdate
from schemas import ReceitaCreate, Receita, ReceitaIngredienteCreate
from models import Receita as ReceitaModel, ReceitaIngrediente as ReceitaIngredienteModel
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

# CRUD de Ingredientes
@app.post("/ingredientes/")
def criar_ingrediente(ingrediente: IngredienteCreate, db: Session = Depends(get_db)):
    try:
        ingrediente_existente = db.query(IngredienteModel).filter(IngredienteModel.nome == ingrediente.nome).first()
        if ingrediente_existente:
            raise HTTPException(status_code=400, detail="Ingrediente já existe.")

        novo_ingrediente = IngredienteModel(nome=ingrediente.nome, unidade=ingrediente.unidade)
        db.add(novo_ingrediente)
        db.commit()
        db.refresh(novo_ingrediente)
        return {"mensagem": "Ingrediente criado com sucesso!", "id": novo_ingrediente.id, "nome": novo_ingrediente.nome, "unidade": novo_ingrediente.unidade}

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
    

@app.put("/ingredientes/{ingrediente_id}")
def atualizar_ingrediente(ingrediente_id: int, dados: IngredienteUpdate, db: Session = Depends(get_db)):
    try:
        ingrediente = db.query(IngredienteModel).filter(IngredienteModel.id == ingrediente_id).first()

        if not ingrediente:
            raise HTTPException(status_code=404, detail="Ingrediente não encontrado.")

        ingrediente.nome = dados.nome
        ingrediente.unidade = dados.unidade

        db.commit()
        db.refresh(ingrediente)

        return {"mensagem": "Ingrediente atualizado com sucesso!", "ingrediente": {
            "id": ingrediente.id,
            "nome": ingrediente.nome,
            "unidade": ingrediente.unidade
        }}

    except SQLAlchemyError as e:
        raise HTTPException(status_code=500, detail=f"Erro no banco: {e}")
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Erro inesperado: {e}")    
    

@app.delete("/ingredientes/{ingrediente_id}")
def deletar_ingrediente(ingrediente_id: int, db: Session = Depends(get_db)):
    try:
        ingrediente = db.query(IngredienteModel).filter(IngredienteModel.id == ingrediente_id).first()

        if not ingrediente:
            raise HTTPException(status_code=404, detail="Ingrediente não encontrado.")

        db.delete(ingrediente)
        db.commit()

        return {"mensagem": f"Ingrediente com ID {ingrediente_id} deletado com sucesso!"}

    except SQLAlchemyError as e:
        raise HTTPException(status_code=500, detail=f"Erro no banco: {e}")
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Erro inesperado: {e}")


@app.post("/receitas/", response_model=Receita)
def criar_receita(receita: ReceitaCreate, db: Session = Depends(get_db)):
    try:
        nova_receita = ReceitaModel(
            nome=receita.nome,
            modo_preparo=receita.modo_preparo
        )
        db.add(nova_receita)
        db.commit()
        db.refresh(nova_receita)

        for item in receita.ingredientes:
            relacao = ReceitaIngredienteModel(
                receita_id=nova_receita.id,
                ingrediente_id=item.ingrediente_id,
                quantidade=item.quantidade
            )
            db.add(relacao)

        db.commit()
        db.refresh(nova_receita)

        return nova_receita

    except SQLAlchemyError as e:
        raise HTTPException(status_code=500, detail=f"Erro no banco: {e}")
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Erro inesperado: {e}")