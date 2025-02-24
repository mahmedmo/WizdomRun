from flask import Blueprint, request, jsonify
from app.extensions import db
from app.models import PlayerCharacter

characters_bp = Blueprint("characters", __name__)

@characters_bp.route("/create", methods=["POST"])
def create_character():
    data = request.get_json()
    if not all(key in data for key in ["userID", "modelID"]):  # modelID is required, others are optional
        return jsonify({"error": "Missing required fields"}), 400

    new_character = PlayerCharacter(
        userID=data["userID"],
        modelID=data["modelID"],
        hairID=data.get("hairID"),
        robeID=data.get("robeID"),
        bootID=data.get("bootID")
    )
    db.session.add(new_character)
    db.session.commit()

    return jsonify({
        "characterID": new_character.characterID,
        "userID": new_character.userID,
        "modelID": new_character.modelID,
        "hairID": new_character.hairID,
        "robeID": new_character.robeID,
        "bootID": new_character.bootID
    }), 201

@characters_bp.route("/<int:userID>", methods=["GET"])
def get_character(userID):
    character = PlayerCharacter.query.filter_by(userID=userID).first()
    if not character:
        return jsonify({"error": "Character not found"}), 404

    return jsonify({
        "characterID": character.characterID,
        "userID": character.userID,
        "modelID": character.modelID,
        "hairID": character.hairID,
        "robeID": character.robeID,
        "bootID": character.bootID
    })

@characters_bp.route("/update/<int:characterID>", methods=["PUT"])
def update_character(characterID):
    data = request.get_json()
    character = PlayerCharacter.query.get(characterID)

    if not character:
        return jsonify({"error": "Character not found"}), 404

    if "modelID" in data:
        character.modelID = data["modelID"]
    if "hairID" in data:
        character.hairID = data["hairID"]
    if "robeID" in data:
        character.robeID = data["robeID"]
    if "bootID" in data:
        character.bootID = data["bootID"]

    db.session.commit()
    return jsonify({"message": "Character updated successfully"})
