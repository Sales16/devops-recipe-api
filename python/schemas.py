from pydantic import BaseModel

class IngredienteCreate(BaseModel):
    nome: str
    unidade: str
    
class Ingrediente(IngredienteCreate):
    id: int

    class Config:
        orm_mode = True

class IngredienteUpdate(BaseModel):
    nome: str
    unidade: str