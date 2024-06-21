import { GetInputStatistics } from '@lib/smAPI/Statistics/StatisticsCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetInputStatistics = createAsyncThunk('cache/getGetInputStatistics', async (_: void, thunkAPI) => {
  try {
    console.log('Fetching GetInputStatistics');
    const response = await GetInputStatistics();
    console.log('Fetched GetInputStatistics ',response?.length);
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


