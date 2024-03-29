import { GetIsSystemReady } from '@lib/smAPI/Settings/SettingsCommands';
import { GetSettings } from '@lib/smAPI/Settings/SettingsCommands';
import { GetSystemStatus } from '@lib/smAPI/Settings/SettingsCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';

export const fetchGetIsSystemReady = createAsyncThunk('cache/getGetIsSystemReady', async (_: void, thunkAPI) => {
  try {
    const response = await GetIsSystemReady();
    return { value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ value: undefined, error: error || 'Unknown error' });
  }
});

export const fetchGetSettings = createAsyncThunk('cache/getGetSettings', async (_: void, thunkAPI) => {
  try {
    const response = await GetSettings();
    return { value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ value: undefined, error: error || 'Unknown error' });
  }
});

export const fetchGetSystemStatus = createAsyncThunk('cache/getGetSystemStatus', async (_: void, thunkAPI) => {
  try {
    const response = await GetSystemStatus();
    return { value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ value: undefined, error: error || 'Unknown error' });
  }
});

