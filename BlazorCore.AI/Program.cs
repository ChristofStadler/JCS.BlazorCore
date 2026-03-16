using GeneticSharp;
using System;
using System.Diagnostics.CodeAnalysis;

namespace BlazorCore.AI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var selection = new RouletteWheelSelection();
            var crossover = new OrderedCrossover();
            var mutation = new ReverseSequenceMutation();
            var fitness = new MyProblemFitness();
            var chromosome = new MyProblemChromosome();
            var population = new Population(50, 70, chromosome);

            var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
            ga.Termination = new GenerationNumberTermination(100);

            Console.WriteLine("GA running...");
            ga.Start();

            Console.WriteLine("Best solution found has {0} fitness.", ga.BestChromosome.Fitness);
        }

        public class MyProblemChromosome : ChromosomeBase
        {
            public MyProblemChromosome() : base(10)
            {
                CreateGenes();
            }

            public override Gene GenerateGene(int geneIndex)
            {
                return new Gene(geneIndex);
            }

            public override IChromosome CreateNew()
            {
                return new MyProblemChromosome();
            }
        }

        public class MyProblemFitness : IFitness
        {
            public double Evaluate(IChromosome chromosome)
            {
                return 0.0;
            }
        }

        public class AI
        {
            public class Environment
            {
                public int[] Observation()
                {
                    var state = new int[8];
                    state[0] = -1; // Action: -1 Left, 0 Forward, 1 Right
                    state[1] = 1; // Left Open: 0 False / 1 True
                    state[2] = 1; // Front Open
                    state[3] = 1; // Right Open
                    state[4] = 1; // Enemy Angle
                    state[5] = 1; // Enemy Direction
                    state[6] = 1; // Pickup Angle
                    state[7] = 0; // Pickup Spawn: 0 False / True

                    return state;
                }
            }
        }
    }
}
