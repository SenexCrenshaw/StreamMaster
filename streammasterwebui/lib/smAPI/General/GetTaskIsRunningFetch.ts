import { GetTaskIsRunning } from '@lib/smAPI/General/GeneralCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetTaskIsRunning = createAsyncThunk('cache/getGetTaskIsRunning', async (_: void, thunkAPI) => {
  try {
    const response = await GetTaskIsRunning();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


