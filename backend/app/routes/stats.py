from flask import Blueprint, request, jsonify
from app.extensions import db
from app.models import PlayerStats, PlayerSpells, Spell

stats_bp = Blueprint("stats", __name__)

@stats_bp.route("/<int:campaignID>", methods=["GET"])
def get_player_stats(campaignID):
    stats = PlayerStats.query.get(campaignID)
    if not stats:
        return jsonify({"error": "Player stats not found"}), 404

    return jsonify({
        "campaignID": stats.campaignID,
        "attack": stats.attack,
        "hp": stats.hp,
        "mana": stats.mana,
        "affinity": stats.affinity
    })

@stats_bp.route("/update/<int:campaignID>", methods=["PUT"])
def update_player_stats(campaignID):
    data = request.get_json()
    stats = PlayerStats.query.get(campaignID)

    if not stats:
        return jsonify({"error": "Player stats not found"}), 404

    if "attack" in data:
        stats.attack = data["attack"]
    if "hp" in data:
        stats.hp = data["hp"]
    if "mana" in data:
        stats.mana = data["mana"]

    db.session.commit()
    return jsonify({"message": "Player stats updated successfully"})

@stats_bp.route("/spells", methods=["GET"])
def get_spells():
    spells = Spell.query.all()
    return jsonify([{"spellID": s.spellID, "name": s.spellName, "element": s.spellElement} for s in spells])

@stats_bp.route("/assign_spell", methods=["POST"])
def assign_spell():
    data = request.get_json()
    if not all(key in data for key in ["campaignID", "spellID"]):
        return jsonify({"error": "Missing required fields"}), 400

    new_spell = PlayerSpells(playerID=data["campaignID"], spellID=data["spellID"])
    db.session.add(new_spell)
    db.session.commit()

    return jsonify({"message": "Spell assigned successfully"})

@stats_bp.route("/player_spells/<int:campaignID>", methods=["GET"])
def get_player_spells(campaignID):
    spells = PlayerSpells.query.filter_by(playerID=campaignID).all()
    
    if not spells:
        return jsonify({"error": "No spells found for this player"}), 404

    return jsonify([
        {
            "playerSpellID": ps.playerSpellID,
            "spellID": ps.spellID,
            "spellName": ps.spell.spellName,  # Assuming a relationship exists between PlayerSpells and Spells
            "spellElement": ps.spell.spellElement
        }
        for ps in spells
    ])
