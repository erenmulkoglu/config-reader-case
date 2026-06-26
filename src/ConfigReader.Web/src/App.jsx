import { useEffect, useMemo, useState } from "react";
import axios from "axios";
import "./App.css";

const API_URL = "http://localhost:7193/api/configurations";

function App() {
    const [items, setItems] = useState([]);
    const [filter, setFilter] = useState("");
    const [form, setForm] = useState({
        name: "",
        type: "string",
        value: "",
        isActive: true,
        applicationName: "SERVICE-A"
    });

    const filteredItems = useMemo(() => {
        return items.filter(x =>
            x.name.toLowerCase().includes(filter.toLowerCase())
        );
    }, [items, filter]);

    const loadItems = async () => {
        const response = await axios.get(API_URL);
        setItems(response.data);
    };

    useEffect(() => {
        loadItems();
    }, []);

    const createItem = async (e) => {
        e.preventDefault();
        await axios.post(API_URL, form);
        setForm({
            name: "",
            type: "string",
            value: "",
            isActive: true,
            applicationName: "SERVICE-A"
        });
        await loadItems();
    };

    const updateItem = async (item) => {
        await axios.put(`${API_URL}/${item.id}`, item);
        await loadItems();
    };

    return (
        <div className="container">
            <h1>Dynamic Configuration Management</h1>

            <form className="card" onSubmit={createItem}>
                <h2>Yeni Konfigürasyon</h2>

                <input
                    placeholder="Name"
                    value={form.name}
                    onChange={e => setForm({ ...form, name: e.target.value })}
                />

                <select
                    value={form.type}
                    onChange={e => setForm({ ...form, type: e.target.value })}
                >
                    <option value="string">string</option>
                    <option value="int">int</option>
                    <option value="double">double</option>
                    <option value="bool">bool</option>
                </select>

                <input
                    placeholder="Value"
                    value={form.value}
                    onChange={e => setForm({ ...form, value: e.target.value })}
                />

                <input
                    placeholder="Application Name"
                    value={form.applicationName}
                    onChange={e => setForm({ ...form, applicationName: e.target.value })}
                />

                <label>
                    <input
                        type="checkbox"
                        checked={form.isActive}
                        onChange={e => setForm({ ...form, isActive: e.target.checked })}
                    />
                    Active
                </label>

                <button type="submit">Ekle</button>
            </form>

            <div className="card">
                <h2>Konfigürasyon Listesi</h2>

                <input
                    placeholder="Client-side name filter"
                    value={filter}
                    onChange={e => setFilter(e.target.value)}
                />

                <table>
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Type</th>
                            <th>Value</th>
                            <th>Active</th>
                            <th>Application</th>
                            <th>Version</th>
                            <th>Action</th>
                        </tr>
                    </thead>

                    <tbody>
                        {filteredItems.map(item => (
                            <tr key={item.id}>
                                <td>
                                    <input
                                        value={item.name}
                                        onChange={e =>
                                            setItems(items.map(x =>
                                                x.id === item.id ? { ...x, name: e.target.value } : x
                                            ))
                                        }
                                    />
                                </td>
                                <td>
                                    <select
                                        value={item.type}
                                        onChange={e =>
                                            setItems(items.map(x =>
                                                x.id === item.id ? { ...x, type: e.target.value } : x
                                            ))
                                        }
                                    >
                                        <option value="string">string</option>
                                        <option value="int">int</option>
                                        <option value="double">double</option>
                                        <option value="bool">bool</option>
                                    </select>
                                </td>
                                <td>
                                    <input
                                        value={item.value}
                                        onChange={e =>
                                            setItems(items.map(x =>
                                                x.id === item.id ? { ...x, value: e.target.value } : x
                                            ))
                                        }
                                    />
                                </td>
                                <td>
                                    <input
                                        type="checkbox"
                                        checked={item.isActive}
                                        onChange={e =>
                                            setItems(items.map(x =>
                                                x.id === item.id ? { ...x, isActive: e.target.checked } : x
                                            ))
                                        }
                                    />
                                </td>
                                <td>
                                    <input
                                        value={item.applicationName}
                                        onChange={e =>
                                            setItems(items.map(x =>
                                                x.id === item.id ? { ...x, applicationName: e.target.value } : x
                                            ))
                                        }
                                    />
                                </td>
                                <td>{item.version}</td>
                                <td>
                                    <button onClick={() => updateItem(item)}>Güncelle</button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
}

export default App;
