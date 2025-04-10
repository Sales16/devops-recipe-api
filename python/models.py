from sqlalchemy import Column, Integer, String
from database import Base

class Ingrediente(Base):
    __tablename__ = "ingredientes"

    id = Column(Integer, primary_key=True, index=True)
    nome = Column(String, unique=True, index=True)
    unidade = Column(String)
