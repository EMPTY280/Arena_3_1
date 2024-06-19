using System.Collections.Generic;
using UnityEngine;

namespace ArenaGame
{
    public class EntityHitParticle : ArenaBaseObjectMono
    {
        private struct ParticleRequest
        {
            public Vector2 pos;
            public Color color;

            public ParticleRequest(Vector2 pos, Color color)
            {
                this.pos = pos;
                this.color = color;
            }
        }

        private ParticleSystem ps = null;
        private ParticleSystem.MainModule main;
        private Queue<ParticleRequest> requests = new Queue<ParticleRequest>();


        private void Awake()
        {
            ps = GetComponent<ParticleSystem>();
            main = ps.main;
        }

        private void Update()
        {
            if (requests.Count < 1) return;

            ParticleRequest req = requests.Dequeue();
            transform.position = req.pos;
            ParticleSystem.MinMaxGradient newColor = new(Color.white, req.color);
            main.startColor = newColor;
            ps.Play();
        }

        public void Play(Vector2 pos, Color color)
        {
            requests.Enqueue(new ParticleRequest(pos, color));
        }
    }
}