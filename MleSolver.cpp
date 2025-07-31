#include "MleSolver.h"
#include <cmath>      // For std::log, std::pow, std::sqrt
#include <limits>     // For std::numeric_limits

// Define M_PI if it's not available (common on Windows with MSVC)
#ifndef M_PI
#define M_PI 3.14159265358979323846
#endif

// This is the core logic from your original main() function, now as a class method.
std::pair<double, double> MleSolver::fit(const std::vector<double>& data) {
    double best_mu = 0.0;
    double best_sigma = 0.0;
    double max_log_likelihood = -std::numeric_limits<double>::infinity();
    size_t n = data.size();

    // Simple grid search for mu and sigma
    for (double mu = 0; mu < 15; mu += 0.1) {
        for (double sigma = 0.1; sigma < 10; sigma += 0.1) {
            
            // Calculate Log-Likelihood for the current mu and sigma
            double current_log_likelihood = 0.0;
            double term1 = -n * 0.5 * std::log(2 * M_PI);
            double term2 = -n * std::log(sigma);
            double term3_sum = 0.0;

            for (const auto& x : data) {
                term3_sum += std::pow(x - mu, 2);
            }
            double term3 = -term3_sum / (2 * std::pow(sigma, 2));
            current_log_likelihood = term1 + term2 + term3;

            // Check if this is the best combination we've found so far
            if (current_log_likelihood > max_log_likelihood) {
                max_log_likelihood = current_log_likelihood;
                best_mu = mu;
                best_sigma = sigma;
            }
        }
    }
    return { best_mu, best_sigma };
}