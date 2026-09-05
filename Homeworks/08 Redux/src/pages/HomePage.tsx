import React, { useState } from 'react';
import { PageTitle } from '../components';
import { Box, Button, List, ListItem } from '@mui/material';
import { TextFieldInput } from '../components';
import { useAppSelector } from '../store/hooks';

interface Task {
    id: number;
    title: string;
    description: string;
    userFullName: string;
    completed: boolean;
}

export const HomePage: React.FC = () => {
    const [tasks, setTasks] = useState<Task[]>([]);
    const [newTask, setNewTask] = useState<Task>({
        id: 0,
        title: '',
        description: '',
        userFullName: '',
        completed: false,
    });
    const { user } = useAppSelector((state) => state.auth);

    return (
        <>
            <PageTitle title="Список задач" />
            <Box
                component="form"
                onSubmit={(e) => {
                    e.preventDefault();
                }}
                sx={{ display: 'flex', flexDirection: 'column', gap: 2, maxWidth: 400 }}
            >
                <TextFieldInput
                    label="Новая задача"
                    type="text"
                    value={newTask.title}
                    onChange={(e) => setNewTask({ ...newTask, title: e.target.value })}
                    required
                />
                <TextFieldInput
                    label="Описание"
                    type="text"
                    value={newTask.description}
                    onChange={(e) => setNewTask({ ...newTask, description: e.target.value })}
                    required
                />
                <Button
                    variant="contained"
                    color="primary"
                    onClick={() => {
                        setNewTask({ ...newTask, userFullName: user?.lastName + ' ' + user?.firstName })
                        setTasks([...tasks, newTask])
                    }}
                >Добавить</Button>
            </Box>

            <List>
                {tasks.map((task) => {
                    return (
                        <>
                            <ListItem>
                                {task.title}
                            </ListItem>
                            <ListItem>
                                {task.description}
                            </ListItem>
                            <ListItem>
                                {task.userFullName}
                            </ListItem>
                        </>
                    )
                })}
            </List>
        </>
    );
}
