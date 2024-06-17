import { GetSMTasks } from '@lib/smAPI/SMTasks/SMTasksCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetSMTasks = createAsyncThunk('cache/getGetSMTasks', async (_: void, thunkAPI) => {
  try {
    console.log('Fetching GetSMTasks');
    const response = await GetSMTasks();
    console.log('Fetched GetSMTasks ',response?.length);
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


