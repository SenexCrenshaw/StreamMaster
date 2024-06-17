import { GetIsSystemReady } from '@lib/smAPI/General/GeneralCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetIsSystemReady = createAsyncThunk('cache/getGetIsSystemReady', async (_: void, thunkAPI) => {
  try {
    console.log('Fetching GetIsSystemReady');
    const response = await GetIsSystemReady();
    console.log('Fetched GetIsSystemReady',response);
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


