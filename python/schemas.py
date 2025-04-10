from pydantic import BaseModel

class IngredienteCreate(BaseModel):
    nome: str
    unidade: str
