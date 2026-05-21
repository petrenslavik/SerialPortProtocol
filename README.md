# Filmobus Serial Protocol Parser

A C# / WPF desktop application built in 2018–2019. The brief came
through a contact at Filmobus, a micro-controllers and robotics
company, who had a Delphi 7 application doing similar work and
wanted to explore a modernised C# version.

This was an unpaid exploratory project. I built the receive side
(serial reading, CRC-16 verification, live plotting) to a working
state; the transmit side (signal generation, two-way comms) was
scaffolded architecturally but I didn't push it through to
completion.

## What works

- **Serial port management** — open, configure, manage USB-COM
  connections
- **Receive-side protocol parsing** — read incoming byte stream
  from microcontroller devices, verify integrity with CRC-16
  checksums
- **Live data plotting** — visualise incoming sensor / data
  streams in a real-time plot window
- **Settings persistence** for port configuration

## What was started but not finished

- **Signal generation modes** (sine, triangle, manual input) are
  architecturally scaffolded but the transmit pipeline wasn't
  completed

## Architecture

WPF MVVM application:
- **Models** — `Packet`, `Level`, `InformationField`, `IMode`;
  signal generators (`Sin`, `Triangle`); `RealTimeManual` mode
- **ViewModels** — `Application`, `Port`, `Reader`, `Sender`,
  `Settings`
- **Views** — `PortView`, `ReaderView`, `SenderView`
- **Windows** — `MainWindow`, `PlotWindow`, `SettingsWindow`
- **HelpfulClasses** — `CRC16`, `RelayCommand`,
  `SerializableData`

## Stack

- C# / .NET Framework
- WPF (XAML, MVVM with `RelayCommand`)
- `System.IO.Ports.SerialPort` for the USB-serial layer
- CRC-16 checksum implementation

## Status

Personal / exploratory work, not maintained. Kept as a portfolio
reference for low-level serial I/O, CRC-16 protocol implementation,
and WPF MVVM patterns from early in my career.
