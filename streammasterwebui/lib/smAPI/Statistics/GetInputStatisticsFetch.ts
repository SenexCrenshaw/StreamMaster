import { GetInputStatistics } from '@lib/smAPI/Statistics/StatisticsCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';

export const fetchGetInputStatistics = createAsyncThunk('cache/getGetInputStatistics', async (_: void, thunkAPI) => {
  try {
    const response = await GetInputStatistics();
    return { param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});
