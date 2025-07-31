#pragma once
#include <vector>
#include <utility> // Required for std::pair

// This class will encapsulate all the logic for our MLE calculation.
class MleSolver {
public:
    // The main function to perform MLE. It takes a vector of data points
    // and returns the estimated mu and sigma as a pair.
    std::pair<double, double> fit(const std::vector<double>& data);
};