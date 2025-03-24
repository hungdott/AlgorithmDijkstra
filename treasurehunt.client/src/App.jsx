import React, { useState, useEffect } from "react";
import { Button, Grid, TextField, Typography, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow } from "@mui/material";
import axios from "axios";

const App = () => {
    const [n, setN] = useState(3);
    const [m, setM] = useState(3);
    const [p, setP] = useState(3);
    const [matrix, setMatrix] = useState(Array(3).fill(Array(3).fill(1)));
    const [fuel, setFuel] = useState(null);
    const [history, setHistory] = useState([]);

    useEffect(() => {
        fetchHistory();
    }, []);

    const fetchHistory = async () => {
        try {
            const response = await axios.get("https://localhost:7203/api/Treasure/history");
            setHistory(response.data);
        } catch (error) {
            console.error(error);
        }
    };

    const handleMatrixChange = (row, col, value) => {
        const newMatrix = [...matrix];
        newMatrix[row] = [...newMatrix[row]];
        newMatrix[row][col] = Math.max(1, Math.min(p, Number(value) || 1));
        setMatrix(newMatrix);
    };

    const handleSolve = async () => {
        try {
            const response = await axios.post("https://localhost:7203/api/Treasure/solve", {
                rowCount: n,
                columnCount: m,
                maxChestNumber: p,
                matrixJson: JSON.stringify(matrix),
            });
            setFuel(response.data.fuelRequired);
            fetchHistory();
        } catch (error) {
            console.error(error);
        }
    };

    return (
        <Paper style={{ padding: 20, maxWidth: 700, margin: "20px auto" }}>
            <Typography variant="h4">Tìm kho báu</Typography>
            <Grid container spacing={1}>
                {[["n", setN], ["m", setM], ["p", setP]].map(([label, setFunc]) => (
                    <Grid item key={label}>
                        <TextField label={label} type="number" value={eval(label)} onChange={(e) => setFunc(Number(e.target.value))} />
                    </Grid>
                ))}
            </Grid>

            <Grid container spacing={1} style={{ marginTop: 10 }}>
                {matrix.map((row, i) =>
                    row.map((cell, j) => (
                        <Grid item key={`${i}-${j}`}>
                            <TextField type="number" value={cell} onChange={(e) => handleMatrixChange(i, j, e.target.value)} />
                        </Grid>
                    ))
                )}
            </Grid>

            <Button variant="contained" color="primary" onClick={handleSolve} style={{ marginTop: 20 }}>
                Giải bài toán
            </Button>

            {fuel !== null && (
                <Typography variant="h6" style={{ marginTop: 10 }}>
                    Lượng nhiên liệu tối thiểu: {fuel}
                </Typography>
            )}

            {/* Bảng lịch sử */}
            <Typography variant="h5" style={{ marginTop: 20 }}>
                Lịch sử bài toán đã giải
            </Typography>
            <TableContainer>
                <Table>
                    <TableHead>
                        <TableRow>
                            <TableCell>N</TableCell>
                            <TableCell>M</TableCell>
                            <TableCell>P</TableCell>
                            <TableCell>Matrix</TableCell>
                            <TableCell>Kết quả</TableCell>
                            <TableCell>Ngày tạo</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {history.map((entry) => (
                            <TableRow>
                                <TableCell>{entry.rowCount}</TableCell>
                                <TableCell>{entry.columnCount}</TableCell>
                                <TableCell>{entry.maxChestNumber}</TableCell>
                                <TableCell>{entry.matrix}</TableCell>
                                <TableCell>{entry.minimumFuelRequired}</TableCell>
                                <TableCell>{new Date(entry.createdTime).toLocaleString()}</TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </TableContainer>
        </Paper>
    );
};

export default App;
