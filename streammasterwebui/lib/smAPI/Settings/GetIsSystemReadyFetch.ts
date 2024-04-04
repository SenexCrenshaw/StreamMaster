import { GetIsSystemReady } from '@lib/smAPI/Settings/SettingsCommands';
import { GetIsSystemReadyRequest } from '../smapiTypes';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetIsSystemReady = createAsyncThunk('cache/getGetIsSystemReady', async (_: void, thunkAPI) => {
  try {
    console.log('Fetching GetIsSystemReady');
    const response = await GetIsSystemReady();
    console.log('Fetched GetIsSystemReady',response);
    return { value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ value: undefined, error: error || 'Unknown error' });
  }
});


