import { GetIsSystemReady } from '@lib/smAPI/Settings/SettingsCommands';
import { GetSettings } from '@lib/smAPI/Settings/SettingsCommands';
import { GetSystemStatus } from '@lib/smAPI/Settings/SettingsCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';
import store from '@lib/redux/store';

export const fetchGetIsSystemReady = createAsyncThunk('cache/getGetIsSystemReady', async (_: void, thunkAPI) => {
  try {
    const test = store.getState().GetIcons;
    if (test.data) return;
    console.log('Fetching GetIsSystemReady');
    const response = await GetIsSystemReady();
    console.log('Fetched GetIsSystemReady ',response?.length);
    return { value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ value: undefined, error: error || 'Unknown error' });
  }
});

export const fetchGetSettings = createAsyncThunk('cache/getGetSettings', async (_: void, thunkAPI) => {
  try {
    const test = store.getState().GetIcons;
    if (test.data) return;
    console.log('Fetching GetSettings');
    const response = await GetSettings();
    console.log('Fetched GetSettings ',response?.length);
    return { value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ value: undefined, error: error || 'Unknown error' });
  }
});

export const fetchGetSystemStatus = createAsyncThunk('cache/getGetSystemStatus', async (_: void, thunkAPI) => {
  try {
    const test = store.getState().GetIcons;
    if (test.data) return;
    console.log('Fetching GetSystemStatus');
    const response = await GetSystemStatus();
    console.log('Fetched GetSystemStatus ',response?.length);
    return { value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ value: undefined, error: error || 'Unknown error' });
  }
});

