#include <iostream>
#include <vector>
#include <random>
#include "MleSolver.h" // Include our new class definition

// Helper function to generate test data
std::vector<double> generate_test_data(int num_samples, double mu, double sigma) {
    std::vector<double> data;
    std::random_device rd;
    std::mt19937 gen(rd());
    std::normal_distribution<> d(mu, sigma);
    for (int i = 0; i < num_samples; ++i) {
        data.push_back(d(gen));
    }
    return data;
}

int main() {
    // 1. Create an instance of our solver.
    MleSolver solver;

    // 2. Generate some test data, just like before.
    std::vector<double> sample_data = generate_test_data(1000, 6.0, 2.0);

    // 3. Use the solver to find the parameters.
    std::pair<double, double> params = solver.fit(sample_data);

    // 4. Print the results.
    std::cout << "--- C++ Refactoring Test ---" << std::endl;
    std::cout << "Estimated mu: " << params.first << std::endl;
    std::cout << "Estimated sigma: " << params.second << std::endl;

    return 0;
}