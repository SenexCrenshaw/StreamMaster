import { GetTaskIsRunning } from '@lib/smAPI/General/GeneralCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetTaskIsRunning = createAsyncThunk('cache/getGetTaskIsRunning', async (_: void, thunkAPI) => {
  try {
    console.log('Fetching GetTaskIsRunning');
    const response = await GetTaskIsRunning();
    console.log('Fetched GetTaskIsRunning',response);
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


