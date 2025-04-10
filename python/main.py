from fastapi import FastAPI, Request
from database import engine, SessionLocal, Base
from models import Ingrediente
from sqlalchemy.exc import SQLAlchemyError

Base.metadata.create_all(bind=engine)

app = FastAPI()

@app.post("/ingredientes/")
async def criar_ingrediente(request: Request):
    try:
        dados = await request.json()
        nome = dados.get("nome")
        unidade = dados.get("unidade")

        if not nome or not unidade:
            return {"erro": "Nome e unidade são obrigatórios."}

        db = SessionLocal()

        ingrediente_existente = db.query(Ingrediente).filter(Ingrediente.nome == nome).first()
        if ingrediente_existente:
            return {"erro": "Ingrediente já existe."}

        novo_ingrediente = Ingrediente(nome=nome, unidade=unidade)
        db.add(novo_ingrediente)
        db.commit()
        db.refresh(novo_ingrediente)
        db.close()

        return {"mensagem": "Ingrediente criado com sucesso!", "ingrediente": {"id": novo_ingrediente.id, "nome": novo_ingrediente.nome, "unidade": novo_ingrediente.unidade}}

    except SQLAlchemyError as e:
        return {"erro": f"Erro no banco de dados: {str(e)}"}
    except Exception as e:
        return {"erro": f"Erro inesperado: {str(e)}"}
