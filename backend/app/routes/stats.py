from flask import Blueprint, request, jsonify
from app.extensions import db
from app.models import PlayerStats, PlayerSpells, Spell
from ..firebase_auth import verify_firebase_token

stats_bp = Blueprint("stats", __name__)

@stats_bp.route("/<int:campaignID>", methods=["GET"])
@verify_firebase_token
def get_player_stats(user, campaignID):
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
@verify_firebase_token
def update_player_stats(user, campaignID):
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
    if "affinity" in data:
        stats.affinity = data["affinity"]

    db.session.commit()
    return jsonify({"message": "Player stats updated successfully"})

@stats_bp.route("/spells", methods=["GET"])
@verify_firebase_token
def get_spells(user):
    spells = Spell.query.all()
    return jsonify([{"spellID": s.spellID, "name": s.spellName, "element": s.spellElement} for s in spells])

@stats_bp.route("/assign_spells", methods=["POST"])
@verify_firebase_token
def assign_spells(user):
    data = request.get_json()
    
    if "campaignID" not in data or "spellIDs" not in data or not isinstance(data["spellIDs"], list) or len(data["spellIDs"]) != 4:
        return jsonify({"error": "Request must contain 'campaignID' and exactly 4 'spellIDs'"}), 400
    
    campaignID = data["campaignID"]
    new_spell_ids = set(data["spellIDs"])
    
    PlayerSpells.query.filter_by(playerID=campaignID).delete()

    new_spells = [PlayerSpells(playerID=campaignID, spellID=spell_id) for spell_id in new_spell_ids]
    db.session.bulk_save_objects(new_spells)
    db.session.commit()

    return jsonify({"message": "Spells assigned successfully"})


@stats_bp.route("/player_spells/<int:campaignID>", methods=["GET"])
@verify_firebase_token
def get_player_spells(user, campaignID):
    spells = db.session.query(PlayerSpells, Spell).join(Spell, PlayerSpells.spellID == Spell.spellID).filter(PlayerSpells.playerID == campaignID).all()
    
    if not spells:
        return jsonify({"error": "No spells found for this player"}), 404

    return jsonify([
        {
            "playerspellID": ps.playerspellID,
            "spellID": spell.spellID,
            "spellName": spell.spellName,
            "spellElement": spell.spellElement
        }
        for ps, spell in spells
    ])


@stats_bp.route("/create", methods=["POST"])
@verify_firebase_token
def create_player_stats(user):
    data = request.get_json()
    new_stats = PlayerStats(
        campaignID=data["campaignID"],
        attack=data["attack"],
        hp=data["hp"],
        mana=data["mana"],
        affinity=data.get("affinity")
    )
    db.session.add(new_stats)
    db.session.commit()
    return jsonify({"message": "Player stats created successfully"})

@stats_bp.route("/player_spells/delete/<int:playerSpellID>", methods=["DELETE"])
@verify_firebase_token
def delete_player_spell(user, playerSpellID):
    spell = PlayerSpells.query.get(playerSpellID)
    if not spell:
        return jsonify({"error": "Spell not found"}), 404

    db.session.delete(spell)
    db.session.commit()
    return jsonify({"message": "Spell removed successfully"})

@stats_bp.route("/spells/create", methods=["POST"])
@verify_firebase_token
def create_spell(user):
    data = request.get_json()
    new_spell = Spell(
        spellName=data["spellName"],
        description=data["description"],
        spellElement=data["spellElement"]
    )
    db.session.add(new_spell)
    db.session.commit()
    return jsonify({"message": "Spell created successfully"})

@stats_bp.route("/replenish_mana/<int:campaignID>", methods=["PATCH"])
@verify_firebase_token
def replenish_mana(user, campaignID):
    data = request.get_json()
    
    if "manaAmount" not in data:
        return jsonify({"error": "Missing manaAmount"}), 400

    player_stats = PlayerStats.query.get(campaignID)
    if not player_stats:
        return jsonify({"error": "Player stats not found"}), 404

    player_stats.mana = min(player_stats.mana + data["manaAmount"], 100)
    db.session.commit()
    
    return jsonify({"message": "Mana replenished successfully", "newMana": player_stats.mana})