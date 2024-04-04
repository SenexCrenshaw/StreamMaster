import { GetSystemStatus } from '@lib/smAPI/Settings/SettingsCommands';
import { GetSystemStatusRequest } from '../smapiTypes';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetSystemStatus = createAsyncThunk('cache/getGetSystemStatus', async (_: void, thunkAPI) => {
  try {
    console.log('Fetching GetSystemStatus');
    const response = await GetSystemStatus();
    console.log('Fetched GetSystemStatus',response);
    return { value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ value: undefined, error: error || 'Unknown error' });
  }
});


