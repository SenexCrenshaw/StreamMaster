import { GetEPGColors } from '@lib/smAPI/EPG/EPGCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetEPGColors = createAsyncThunk('cache/getGetEPGColors', async (_: void, thunkAPI) => {
  try {
    console.log('Fetching GetEPGColors');
    const response = await GetEPGColors();
    console.log('Fetched GetEPGColors ',response?.length);
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


